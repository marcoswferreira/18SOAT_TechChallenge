# 🎬 Roteiro de Gravação — Apresentação em Vídeo (Tech Challenge Fase 1)

> **Projeto**: SIAES — Sistema Integrado de Atendimento e Execução de Serviços  
> **Turma**: FIAP — Pós-Graduação em Software Architecture (18SOAT)  
> **Grupo**: Grupo 21  
> **Tempo Estimado**: **13 minutos e 30 segundos** *(Limite Máximo do Edital: 15 minutos)*  
> **Formato de Entrega**: Vídeo público no YouTube, gravado com áudio claro e demonstração real da solução.

---

## 📋 Checklist Pré-Gravação (Preparação do Ambiente)

Antes de iniciar a gravação (usando OBS Studio, Loom, Clipchamp ou similar), deixe os seguintes aplicativos e abas preparados:

- [ ] **Aba 1 (Capa / Identificação)**: Abrir [`docs/ENTREGA_FASE1.md`](ENTREGA_FASE1.md) no navegador ou VS Code (mostrando Grupo 21, nomes e RMs).
- [ ] **Aba 2 (Miro - OS)**: Board [Criação e Acompanhamento da Ordem de Serviço](https://miro.com/app/board/uXjVHntHpys=/).
- [ ] **Aba 3 (Miro - Estoque)**: Board [Gestão de Peças e Insumos](https://miro.com/app/board/uXjVHrbvJdE=/).
- [ ] **Aba 4 (Swagger UI)**: `http://localhost:8082/swagger` (ou `http://localhost:5180/swagger`).
- [ ] **Aba 5 (SonarCloud)**: [Painel SonarCloud da Branch Main](https://sonarcloud.io/summary/overall?id=marcoswferreira_18SOAT_TechChallenge&branch=main).
- [ ] **Terminal 1 (Aplicação)**: Aberto na raiz do repositório, pronto para demonstrar `docker compose up -d` ou status dos contêineres.
- [ ] **Terminal 2 (Testes)**: Aberto e limpo, pronto para executar `dotnet test SIAES.slnx`.
- [ ] **Insomnia / Thunder Client**: Collection carregada (`docs/insomnia/Insomnia_SIAES_Collection.json` ou `docs/thunder-client/thunder-collection_SIAES.json`).
- [ ] **Resolução de Tela**: Preferencialmente Full HD (1920x1080) com zoom do navegador em 110% para legibilidade do texto.

---

## 🗺️ Mapa de Conformidade com o Edital

| # | Requisito Obrigatório do Edital (Pág. 7 do PDF) | Minutagem no Roteiro |
| :-: | :--- | :---: |
| 1 | **Apresentação da participação e identificação dos integrantes** | `00:00 - 00:40` |
| 2 | **A proposta da solução** | `00:40 - 01:15` |
| 3 | **A arquitetura do projeto e organização do monólito em camadas** | `01:15 - 02:45` |
| 4 | **A escolha e a justificativa do banco de dados (ADR 001)** | `02:45 - 03:45` |
| 5 | **O processo de Event Storming** | `03:45 - 05:15` |
| 6 | **Os principais elementos de DDD utilizados** | `05:15 - 06:45` |
| 7 | **A execução da aplicação** | `06:45 - 07:30` |
| 8 | **A documentação Swagger ou OpenAPI** | `07:30 - 08:00` |
| 9 | **A autenticação das APIs administrativas (JWT / RBAC)** | `08:00 - 08:45` |
| 10 | **O fluxo de criação e acompanhamento de uma Ordem de Serviço** | `08:45 - 09:30` |
| 11 | **A inclusão de serviços, peças e insumos com reserva de estoque** | `09:30 - 10:15` |
| 12 | **A geração e aprovação do orçamento (com notificações)** | `10:15 - 11:00` |
| 13 | **A alteração dos status da OS (Recebida até Entregue) e consultas públicas** | `11:00 - 11:45` |
| 14 | **Os testes automatizados (Domain, Application, Integration)** | `11:45 - 12:30` |
| 15 | **O resultado da análise de vulnerabilidades (SonarCloud)** | `12:30 - 13:15` |
| - | **Commit informado no PDF (`a4c2102`) e Encerramento** | `13:15 - 13:30` |

---

## ⏱️ Roteiro de Gravação Passo a Passo

---

### 🟢 BLOCO 1: Abertura, Identificação e Proposta da Solução

**Tempo estimado**: `00:00` a `01:15` *(Duração: 1m15s)*

#### 🖥️ O que mostrar na tela

1. Exibir na tela o documento [`docs/ENTREGA_FASE1.md`](ENTREGA_FASE1.md) ou um slide de abertura com os dados do projeto.
2. Destaque visual na tabela de identificação e na lista de integrantes do **Grupo 21**.

#### 🎙️ O que falar
>
> *"Olá a todos e bem-vindos à apresentação do projeto **SIAES** — Sistema Integrado de Atendimento e Execução de Serviços, desenvolvido para a Fase 1 do Tech Challenge da Pós-Graduação em Software Architecture da FIAP, Turma 18SOAT.*
>
> *Nós somos o **Grupo 21**, composto por:*
>
> - *Alexandre Martins Placido (RM378588)*
> - *Diego Estefan Conceição de Pádua (RM378740)*
> - *Jayr Rufino de Almeida Junior (RM378740)*
> - *Marcos Welington Ferreira (RM378531)*
> - *Milena Vargas Leonardi (RM378633)*
>
> *A proposta da nossa solução foi concebida para resolver os desafios reais de uma oficina mecânica de médio porte. Hoje, processos operacionais baseados em anotações em papel e planilhas descentralizadas geram furos no controle de estoque, retrabalho em diagnósticos, morosidade na aprovação de orçamentos e completa falta de visibilidade para o cliente.*
>
> *O SIAES entrega uma plataforma transacional, auditável e segura, construída sobre os princípios de **Domain-Driven Design** e **Clean Architecture**, permitindo tanto a gestão operacional interna quanto o acompanhamento em tempo real pelo cliente via API pública."*

---

### 🟢 BLOCO 2: Arquitetura, Organização do Monólito e Banco de Dados

**Tempo estimado**: `01:15` a `03:45` *(Duração: 2m30s)*

#### 🖥️ O que mostrar na tela

1. Abrir o VS Code exibindo os 4 projetos dentro de `src/`: `Domain`, `Application`, `Infrastructure` e `SIAES.API`.
2. Exibir o diagrama de arquitetura C3 ou C2 em [`docs/architecture/`](architecture/) ou [`docs/ddd/architecture.md`](ddd/architecture.md).
3. Abrir rapidamente a **ADR 001** em [`docs/adrs/adr-001-escolha-do-banco-de-dados.md`](adrs/adr-001-escolha-do-banco-de-dados.md).

#### 🎙️ O que falar
>
> *"Para este MVP, adotamos uma arquitetura de **Monólito Modular** baseada nos princípios de **Clean Architecture** e **Ports & Adapters (Arquitetura Hexagonal)**.*
>
> *A solução está dividida em 4 camadas com dependências estritas direcionadas para o centro:*
>
> 1. *O **Domain (`src/Domain`)** é o coração da aplicação. É 100% puro e isolado de frameworks, contendo nossas entidades ricas, regras de negócio, invariantes, objetos de valor e as interfaces de portas.*
> 2. *A camada de **Application (`src/Application`)** orquestra os casos de uso de negócio através de Use Cases atômicos e DTOs, sem conhecer detalhes de banco de dados.*
> 3. *A camada de **Infrastructure (`src/Infrastructure`)** implementa os adaptadores secundários: Entity Framework Core 10, criptografia AES-256 para conformidade com a LGPD, repositórios com divisão de consultas e o serviço de mensageria com o provedor Resend.*
> 4. *A camada de **Presentation (`src/SIAES.API`)** expõe nossa Web API RESTful com versionamento, autenticação JWT, tratamento global de exceções e Swagger interativo.*
>
> *Em relação à persistência, formalizamos a escolha através da **ADR 001**: adotamos o **PostgreSQL 16** com **Entity Framework Core 10**.*
> *A justificativa técnica baseia-se em 3 pilares cruciais para o nosso domínio:*
>
> - *Primeiro: **Garantias ACID e Atomicidade**, indispensáveis para evitar furos no estoque de peças durante reservas e baixas.*
> - *Segundo: **Concorrência Otimista nativa** utilizando o token de versão `xmin` do PostgreSQL, impedindo que duas ordens de serviço reservem simultaneamente a mesma peça do estoque.*
> - *Terceiro: **Integridade Relacional** e alto desempenho em consultas de agregados utilizando `AsSplitQuery()`."*

---

### 🟢 BLOCO 3: Event Storming e Elementos de DDD

**Tempo estimado**: `03:45` a `06:45` *(Duração: 3m00s)*

#### 🖥️ O que mostrar na tela

1. Alternar para a aba do navegador com o board do Miro: [Criação e Acompanhamento da Ordem de Serviço](https://miro.com/app/board/uXjVHntHpys=/).
2. Fazer pan/zoom mostrando os cartões amarelos (Agregados), azuis (Comandos), laranjas (Eventos) e lilases (Políticas).
3. Alternar para o segundo board do Miro: [Gestão de Peças e Insumos](https://miro.com/app/board/uXjVHrbvJdE=/), mostrando a dinâmica de reserva e baixa de estoque.
4. Voltar rapidamente ao VS Code e mostrar as classes `OrdemServico.cs`, `ItemEstoque.cs` e os Value Objects `Documento.cs` e `Placa.cs`.

#### 🎙️ O que falar
>
> *"A modelagem do sistema nasceu de um workshop colaborativo de **Event Storming**, registrado em dois boards públicos no Miro disponíveis na documentação.*
>
> *No primeiro board, mapeamos a linha do tempo completa da **Ordem de Serviço**: desde a triagem do cliente, emissão de comandos como `CriarOrdemServico` e `IniciarDiagnostico`, até a aprovação do orçamento e entrega do veículo.*
>
> *No segundo board, aprofundamos a **Gestão de Peças e Insumos**. Modelamos as políticas de estoque: quando o mecânico inclui uma peça na OS, é emitido o comando `ReservarItemEstoque`. A política valida se há saldo disponível e bloqueia a quantidade imediatamente. Se o orçamento for rejeitado pelo cliente, a política dispara o evento `ReservaEstoqueLiberada` devolvendo o saldo. Apenas na finalização do serviço ocorre o evento `BaixaEstoqueEfetivada`, subtraindo tanto o estoque físico quanto a reserva.*
>
> *Trazendo isso para o código, implementamos os padrões táticos do DDD:*
>
> - *O **Agregado Raiz `OrdemServico`**, que encapsula uma Máquina de Estados Finita estrita, impedindo saltos ilegais de status (por exemplo, de Recebida direto para Finalizada).*
> - *O **Agregado Raiz `ItemEstoque`**, que separa estritamente `QuantidadeEmEstoque`, `QuantidadeReservada` e calcula dinamicamente a `QuantidadeDisponivel`.*
> - *Os **Value Objects imutáveis**: `Documento`, que executa a validação matemática dos 11 dígitos do CPF ou 14 do CNPJ e provê máscara; e `Placa`, validando formatos antigo e Mercosul.*
> - *E para auditoria, o Value Object `HistoricoStatus`, que registra de forma imutável cada transição de status com data UTC e motivo."*

---

### 🟢 BLOCO 4: Execução Local, Swagger e Autenticação JWT

**Tempo estimado**: `06:45` a `08:45` *(Duração: 2m00s)*

#### 🖥️ O que mostrar na tela

1. Terminal exibindo os contêineres Docker ativos (`docker ps` ou o comando de subida `docker compose up -d`).
2. Navegador abrindo `http://localhost:8082/swagger` (Swagger UI da API SIAES).
3. Executar o endpoint `POST /api/v1/Auth/login` com as credenciais do Admin (`admin@siaes.com` / `SenhaAdmin1!`).
4. Copiar o token JWT retornado, clicar no botão **Authorize** no topo do Swagger e colar o Bearer Token.

#### 🎙️ O que falar
>
> *"Vamos agora para a demonstração prática da solução rodando.*
>
> *A execução local é extremamente simples: basta um único comando `docker compose up -d`. O Docker orquestra o contêiner da Web API .NET 10 e o banco PostgreSQL 16. No primeiro arranque em ambiente de desenvolvimento, a API executa automaticamente as migrações do banco e um **DatabaseSeeder**, que já popula o sistema com usuários de demonstração, clientes, veículos, catálogo de serviços e peças com saldo físico.*
>
> *Ao acessar a documentação interativa via Swagger UI, temos todos os contratos e endpoints categorizados e documentados.*
>
> *A segurança das operações administrativas é protegida por **JSON Web Tokens (JWT)** com suporte a Refresh Tokens em cookies `HttpOnly` e controle de acesso baseado em papéis (**RBAC**).*
> *Fazemos a autenticação na rota `POST /api/v1/Auth/login` com o usuário administrador. Copiamos o `accessToken` gerado e aplicamos no botão Authorize do Swagger. A partir de agora, as requisições autenticadas carregarão a identidade do operador com perfil `Admin`."*

---

### 🟢 BLOCO 5: Demonstração do Fluxo Completo da Ordem de Serviço

**Tempo estimado**: `08:45` a `11:45` *(Duração: 3m00s)*

#### 🖥️ O que mostrar na tela

Executar a sequência de requisições no Swagger ou através da collection do Insomnia / Thunder Client:

1. `POST /api/v1/ordens-servico` (Criar OS com clienteId e veiculoId do seed) ➔ Retorna Status `Recebida`.
2. `POST .../{id}/diagnostico` ➔ Retorna Status `EmDiagnostico`.
3. `POST .../{id}/servicos` (Mão de obra: Troca de óleo - R$ 120,00).
4. `POST .../{id}/pecas` (Peça: Filtro de óleo - 2 un a R$ 50,00) ➔ Mostrar no console/resposta a reserva de estoque.
5. `POST .../{id}/orcamento` ➔ Retorna Status `AguardandoAprovacao` e valor total calculado (R$ 220,00). Mencionar envio de e-mail via Resend.
6. **Consultas Públicas de Status (Sem Token)**:
   - Fazer `GET .../{id}/status` sem passar token Authorization ➔ Retorna 200 OK com status e histórico.
   - Fazer `GET .../documento/12345678909/status` sem token ➔ Retorna as OS do cliente.
7. `POST .../{id}/aprovar` ➔ Retorna Status `EmExecucao`.
8. `POST .../{id}/finalizar` ➔ Retorna Status `Finalizada` (destacar que ocorreu a baixa definitiva no estoque físico).
9. `POST .../{id}/entregar` ➔ Retorna Status `Entregue`.
10. `GET /api/v1/ordens-servico/metricas/tempo-medio-execucao` ➔ Exibir tempo médio de execução calculado.

#### 🎙️ O que falar
>
> *"Vamos executar o fluxo completo de uma Ordem de Serviço na prática.*
>
> *1. Criamos a Ordem de Serviço vinculando o cliente e o veículo. A OS nasce com o status inicial **Recebida**.*
>
> *2. O mecânico inicia a inspeção técnica acionando o endpoint `/diagnostico`. O status transiciona para **EmDiagnostico**.*
>
> *3. Durante o diagnóstico, o mecânico adiciona um serviço de mão de obra do catálogo no valor de R$ 120,00.*
>
> *4. Em seguida, adiciona duas unidades de filtro de óleo a R$ 50,00 cada. Aqui entra a regra de consistência de estoque: a quantidade disponível do item é imediatamente reservada, garantindo que outra equipe não utilize essas peças.*
>
> *5. Com peças e serviços definidos, acionamos o endpoint `/orcamento`. O sistema calcula automaticamente o valor total da OS — no caso, R$ 220,00 —, transiciona o status para **AguardandoAprovacao** e dispara uma notificação por e-mail com layout HTML responsivo via **Resend** para o cliente aprovar.*
>
> *6. Um dos grandes diferenciais do SIAES é a transparência: os endpoints `/status` por ID da OS ou por CPF do cliente possuem o atributo `[AllowAnonymous]`. Ou seja, o cliente final pode consultar pelo celular o andamento do seu veículo em tempo real sem precisar de login ou credencial de acesso.*
>
> *7. O cliente aprova o orçamento pelo endpoint `/aprovar`, mudando o status para **EmExecucao**.*
>
> *8. Ao concluir o reparo, executamos `/finalizar`. O status muda para **Finalizada** e o sistema processa a baixa física atômica: as 2 unidades que estavam reservadas são definitivamente descontadas do estoque físico.*
>
> *9. Quando o cliente retira o carro, registramos `/entregar`, encerrando o atendimento com status **Entregue**.*
>
> *10. Por fim, o endpoint de métricas `/metricas/tempo-medio-execucao` consolida o tempo médio de execução de todas as ordens finalizadas na oficina, fornecendo KPIs para a gerência.*
>
> *(Nota técnica: O sistema também suporta o fluxo alternativo: durante a execução é possível solicitar aprovação adicional, e caso o cliente rejeite um orçamento, a OS é cancelada e a reserva das peças no estoque é estornada automaticamente)."*

---

### 🟢 BLOCO 6: Testes Automatizados e Análise de Vulnerabilidades

**Tempo estimado**: `11:45` a `13:15` *(Duração: 1m30s)*

#### 🖥️ O que mostrar na tela

1. Alternar para o Terminal limpo e rodar o comando:

   ```bash
   dotnet test SIAES.slnx
   ```

   Aguardar os segundos de execução e destacar o resultado final: **414 testes passando, 0 falhas**.
2. Alternar para a aba do navegador com o painel do **SonarCloud (Branch `main`)**.
3. Destacar com o cursor:
   - Quality Gate: **Passed**
   - Cobertura Geral: **84.9%** (superando os 80% mínimos)
   - Cobertura dos domínios críticos: **100%**
   - Vulnerabilidades de Segurança: **0 (Rating A)**
   - Bugs e Confiabilidade: **0 (Rating A)**
   - Duplicações: **0.0%**

#### 🎙️ O que falar
>
> *"No que tange à qualidade de software e garantia de funcionamento, desenvolvemos uma suíte com **414 testes automatizados** distribuídos em três projetos:*
>
> - *Testes unitários de domínio: validando entidades, Value Objects e regras da máquina de estados;*
> - *Testes unitários de aplicação: cobrindo cada caso de uso e DTO;*
> - *E testes de integração: testando os controllers REST e o repositório com o WebApplicationFactory.*
>
> *Como podemos ver no terminal, todos os 414 testes estão passando com 100% de sucesso.*
>
> *Para validação estrita de segurança e qualidade, nosso pipeline de CI/CD no GitHub Actions integra a análise estática contínua do **SonarCloud** na branch `main`.*
> *O resultado da análise comprova:*
>
> - *Quality Gate **Passed**;*
> - *Cobertura geral de código em **84.9%**, superando os 80% exigidos pelo edital, atingindo **100% de cobertura nos arquivos e domínios críticos**;*
> - *Zero vulnerabilidades de segurança (Rating A);*
> - *Zero bugs de confiabilidade (Rating A);*
> - *Zero hotspots de segurança;*
> - *E zero por cento de duplicação de código.*
>
> *Além disso, executamos no pipeline o `dotnet list package --vulnerable`, garantindo que nenhuma dependência com vulnerabilidades conhecidas esteja em uso."*

---

### 🟢 BLOCO 7: Commit da Entrega e Encerramento

**Tempo estimado**: `13:15` a `13:30` *(Duração: 0m15s)*

#### 🖥️ O que mostrar na tela

1. Terminal exibindo `git log -1` na branch `main`:

   ```bash
   git log -1 --oneline
   # Exibindo o commit informado no PDF (ex.: a4c2102)
   ```

2. Voltar à tela inicial ou ao documento [`docs/ENTREGA_FASE1.md`](ENTREGA_FASE1.md).

#### 🎙️ O que falar
>
> *"Toda a demonstração realizada neste vídeo corresponde rigorosamente à versão da branch `main` no commit **`a4c2102`**, conforme informado no nosso documento oficial de entrega em PDF.*
>
> *Agradecemos a atenção dos professores e da banca avaliadora da FIAP. O repositório e todos os artefatos de documentação estão disponíveis publicamente no GitHub para revisão detalhada. Muito obrigado!"*

---

## 📌 Checklist Pós-Gravação (Antes de Enviar o PDF)

- [ ] **Verificar a Duração**: Certifique-se de que o vídeo final tenha menos de 15 minutos (o roteiro foi planejado para ~13m30s).
- [ ] **Conferir o Áudio**: Certifique-se de que a voz está nítida e sem cortes bruscos.
- [ ] **Upload no YouTube**:
  - Configurar a visibilidade como **Público** (ou **Não listado / Unlisted**). *Nunca como "Privado", pois a banca não conseguirá avaliar.*
- [ ] **Atualizar o Link no PDF de Entrega**:
  - Abrir [`docs/ENTREGA_FASE1.md`](ENTREGA_FASE1.md), localizar a Seção 3 (`Link do Vídeo no YouTube`) e substituir `https://youtube.com/todo` pelo link real do vídeo.
  - Exportar novamente para PDF (`ENTREGA_FASE1.pdf`).
