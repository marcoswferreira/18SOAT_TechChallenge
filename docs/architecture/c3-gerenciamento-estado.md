# Diagrama de Componentes (C3 do Modelo C4) — Gerenciamento de Estado (Status da OS)

Este documento descreve a arquitetura do **Gerenciamento de Estado** da Ordem de Serviço no sistema **SIAES**, apresentando o **Diagrama de Componentes (C3 do modelo C4)** que retrata a Máquina de Estados Finita (FSM), suas regras de transição, invariantes, auditoria imutável e efeitos colaterais.

O modelo segue fielmente a estrutura e a representação de camadas definida no arquivo [`C4_GerenciamentoEstado_Component_C3.drawio`](drawio/C4_GerenciamentoEstado_Component_C3.drawio).

---

## 1. Visão Geral

O gerenciamento de estado da Ordem de Serviço é implementado com foco em **integridade de dados**, **auditoria contínua** e **previsibilidade operacional**:
- **Encapsulamento no Domínio**: O status da OS não pode ser alterado arbitrariamente via setters públicos ou comandos genéricos de update. Qualquer alteração passa por métodos de negócio expressivos no agregado `OrdemServico`.
- **Máquina de Estados Finita (FSM)**: Transições entre estados seguem um grafo direcionado e estrito. Tentativas de violar o fluxo (ex.: aprovar uma OS ainda em `Recebida`) resultam em `DomainException`, traduzida para HTTP `422 Unprocessable Entity`.
- **Auditoria Imutável em Tempo Real**: Cada transição gera um registro indelével na entidade `RegistroStatus` persistido na tabela `OrdensServico_Historico` com carimbo de data/hora UTC (`DateTime.UtcNow`).
- **Efeitos Colaterais Controlados**: Mudanças de estado desencadeiam orquestrações de estoque (reserva, liberação ou baixa física) e notificações automáticas ao cliente por e-mail.

---

## 2. Diagrama C3 — Componentes do Gerenciamento de Estado

```mermaid
flowchart TD
    %% Usuários
    Usuarios["Usuários da Oficina\n[Persons]\nAtendentes, Mecânicos e Administradores"]:::person

    %% Fronteira do Sistema SIAES (API Backend)
    subgraph SIAESBoundary ["SIAES [API Backend — ASP.NET Core]"]

        %% Camada de Apresentação
        subgraph CamadaApresentacao ["Camada Apresentação"]
            ControllerStatus["Controller de Status da OS\n[Status da OS]\nExpõe rotas de comando para avanço\nde status da OS e consultas públicas."]:::component
            GlobalExHandler["ExceptionHandler Global\n[Exception Handler]\nCaptura exceções de transição de status\ninválida e retorna HTTP 422."]:::component
        end

        %% Camada Aplicação
        subgraph CamadaAplicacao ["Camada Aplicação [Container]"]
            CasosUsoTransicao["Casos de Uso de Transição de Estado\n[Status da OS]\nOrquestra as transições de status da OS e\ncoordena efeitos de estoque e notificações."]:::component
        end

        %% Camada Domínio
        subgraph CamadaDominio ["Camada Domínio [Container]"]
            FSM["Máquina de Estados da OS (FSM)\n[Status da OS]\nMétodo MudarStatus no agregado OrdemServico,\nvalida fluxo estrito e gera RegistroStatus."]:::component
        end

        %% Camada Infraestrutura
        subgraph CamadaInfra ["Camada Infraestrutura [Container]"]
            PersistenciaHist["Persistência do Histórico da OS\n[Status da OS]\nPersiste a OS e grava registros imutáveis\nna tabela OrdensServico_Historico."]:::component
            DisparoNotif["Disparo de Notificações de Status\n[Notificação]\nEnvia e-mails transacionais automáticos\npara cada etapa concluída da OS."]:::component
            EFCore["Entity Framework Core\n[Entity Framework]\nMapeamento relacional e auditoria\nindelével de transições de status."]:::component
        end

    end

    %% Banco de Dados Externo
    DB[("SIAES_DB\n[Container: PostgreSQL 16]\nBanco de dados relacional: tabelas\nOrdensServico e OrdensServico_Historico.")]:::db

    %% Provedor Externo
    EmailService["Serviço de envio de e-mails\n[Software System]\nServiço externo (Resend) utilizado para\nenvio de notificações aos clientes."]:::external

    %% Relações e Fluxos
    Usuarios -->|"HTTP / REST\n(JSON, Bearer Token)"| ControllerStatus
    ControllerStatus -->|"Comandos de transição"| CasosUsoTransicao
    ControllerStatus -.->|"Tratamento 422"| GlobalExHandler
    CasosUsoTransicao -->|"Valida e aplica transição"| FSM
    FSM -->|"Persiste estado e histórico"| PersistenciaHist
    CasosUsoTransicao -->|"Dispara notificações"| DisparoNotif
    PersistenciaHist -->|"Operações de dados"| EFCore
    EFCore -->|"SQL / TCP 5432"| DB
    DisparoNotif -->|"API / HTTPS\n(Resend SDK)"| EmailService

    %% Estilos visuais C4 (alinhados ao Draw.io)
    classDef person fill:#083F75,stroke:#06315C,color:#ffffff;
    classDef component fill:#1061B0,stroke:#0D5091,color:#ffffff;
    classDef db fill:#23A2D9,stroke:#0E7DAD,color:#ffffff;
    classDef external fill:#8C8496,stroke:#736782,color:#ffffff;
```

---

## 3. Matriz de Estados e Transições Implementadas

A tabela abaixo documenta todas as transições válidas suportadas pelo modelo de domínio:

| Status Origem (`De`) | Status Destino (`Para`) | Endpoint HTTP | Método no Domínio | Invariantes e Regras de Negócio | Efeito Colateral Associado |
| :--- | :--- | :--- | :--- | :--- | :--- |
| — | `Recebida` | `POST /api/v1/ordens-servico` | `new OrdemServico(clienteId, veiculoId)` | Cliente e veículo devem existir previamente. | Cria primeiro `RegistroStatus(null, Recebida)`. Notifica cliente por e-mail: *"Veículo Recebido"*. |
| `Recebida` | `EmDiagnostico` | `POST /api/v1/ordens-servico/{id}/diagnostico` | `os.IniciarDiagnostico()` | A OS deve estar em `Recebida`. | Permite adição de peças e serviços na OS. |
| `EmDiagnostico` | `AguardandoAprovacao` | `POST /api/v1/ordens-servico/{id}/orcamento` | `os.GerarEEnviarOrcamento()` | **`ValorTotal > 0`**: É obrigatório ter ao menos uma peça ou serviço adicionado. | Notifica cliente por e-mail com a tabela de valores: *"Orçamento Disponível para Aprovação"*. |
| `AguardandoAprovacao` | `EmExecucao` | `POST /api/v1/ordens-servico/{id}/aprovar` | `os.AprovarOrcamento()` | A OS deve estar em `AguardandoAprovacao`. | Notifica cliente por e-mail: *"Orçamento Aprovado"*. |
| `AguardandoAprovacao` | `Cancelada` | `POST /api/v1/ordens-servico/{id}/rejeitar` | `os.RejeitarOrcamento()` | A OS deve estar em `AguardandoAprovacao`. | **Reversão de estoque**: `itemEstoque.LiberarReserva()`. Notifica cliente: *"Orçamento Rejeitado"*. |
| `EmExecucao` | `AguardandoAprovacao` | `POST /api/v1/ordens-servico/{id}/solicitar-aprovacao-adicional` | `os.SolicitarAprovacaoAdicional()` | A OS deve estar em `EmExecucao`. Permite incluir novos reparos imprevistos. | Reenvia e-mail com orçamento atualizado para nova aprovação. |
| `EmExecucao` | `Finalizada` | `POST /api/v1/ordens-servico/{id}/finalizar` | `os.FinalizarServico()` | A OS deve estar em `EmExecucao`. | **Baixa física de estoque**: `itemEstoque.BaixarReserva()`. Notifica cliente: *"Veículo Pronto para Retirada"*. |
| `Finalizada` | `Entregue` | `POST /api/v1/ordens-servico/{id}/entregar` | `os.EntregarVeiculo()` | A OS deve estar em `Finalizada`. | Encerra ciclo de atendimento. Notifica cliente: *"Veículo Entregue"*. |

---

## 4. O Fluxo de Transição no Código do Agregado

O coração da máquina de estados fica em [`src/Domain/Entities/OrdemServico.cs`](../../src/Domain/Entities/OrdemServico.cs):

```csharp
private void MudarStatus(StatusOrdemServico novoStatus)
{
    var statusAnterior = Status;
    Status = novoStatus;
    _historico.Add(new RegistroStatus(Id, statusAnterior, novoStatus, DateTime.UtcNow));
}
```

### Invariantes e Tratamento de Violação

Quando uma transição não é permitida pelas regras de negócio, o método do agregado interrompe a execução imediatamente lançando `DomainException`:

```csharp
public void AprovarOrcamento()
{
    if (Status != StatusOrdemServico.AguardandoAprovacao)
        throw new DomainException(
            $"Não é possível aprovar o orçamento com a OS em '{Status}'.");

    MudarStatus(StatusOrdemServico.EmExecucao);
}
```

O middleware central de exceções (`ExceptionHandler` em [`src/Infrastructure/ExceptionHandler.cs`](../../src/Infrastructure/ExceptionHandler.cs)) intercepta a `DomainException` e escreve a resposta padronizada RFC 7807:

```json
HTTP/1.1 422 Unprocessable Entity
Content-Type: application/problem+json

{
  "type": "DomainException",
  "title": "Regra de negócio violada",
  "status": 422,
  "detail": "Não é possível aprovar o orçamento com a OS em 'Recebida'.",
  "instance": "POST /api/v1/ordens-servico/a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d/aprovar",
  "traceId": "00-f9f910a6e2fb5819de07322f6ad6e8e9-509dd96602a22d08-00",
  "requestId": "0HNPEAGJNCOV2:00000001"
}
```

---

## 5. Rastreabilidade e Persistência do Histórico

A persistência do histórico de status é modelada via **Entity Framework Core 10** mapeada para a tabela relacional `OrdensServico_Historico`:

```mermaid
erDiagram
    OrdensServico ||--o{ OrdensServico_Historico : "possui historico de"

    OrdensServico {
        uuid Id PK
        uuid ClienteId FK
        uuid VeiculoId FK
        int Status
        decimal ValorTotal
    }

    OrdensServico_Historico {
        uuid Id PK
        uuid OrdemServicoId FK
        int De "Status anterior (nullable)"
        int Para "Novo status"
        timestamptz Em "Carimbo UTC"
    }
```

### Características de Auditoria
- **Imutabilidade**: Os registros da tabela `OrdensServico_Historico` são exclusivamente criados (`INSERT`), sem suporte a atualização ou exclusão lógica direta.
- **Relacionamento em Cascata**: Configurado no `OrdemServicoMap` com `HasMany(os => os.Historico).WithOne().HasForeignKey(r => r.OrdemServicoId).OnDelete(DeleteBehavior.Cascade)`.
- **Cálculo de Lead Time e SLA**: Cada entrada armazena o momento exato em que a OS mudou de fase, possibilitando auditorias sobre o tempo que a OS permaneceu em diagnóstico, aguardando aprovação ou em execução.

---

## 6. Consulta Pública Desacoplada

Para permitir que o cliente consulte o andamento do seu veículo sem necessidade de login, o componente `GetStatusOrdemServicoUseCase` consome a projeção otimizada `GetStatusClienteResponse`:
- **Por ID**: `GET /api/v1/ordens-servico/{id}/status`
- **Por CPF/CNPJ**: `GET /api/v1/ordens-servico/documento/{documento}/status`

O retorno traduz os termos técnicos para linguagem amigável ao cliente consumidor (ex.: `EmExecucao` torna-se *"Reparo autorizado e serviço em andamento"*).

---

## 7. Arquivo Draw.io Nativo

O diagrama original editável está disponível em:
- 📁 [`docs/architecture/drawio/C4_GerenciamentoEstado_Component_C3.drawio`](drawio/C4_GerenciamentoEstado_Component_C3.drawio)

