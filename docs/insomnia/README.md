# Suíte de Testes End-to-End (E2E) no Insomnia - SIAES API

Este diretório contém a suíte completa de testes de ponta a ponta gerada para o **Insomnia**.

---

## 📁 Arquivos Disponíveis

| Arquivo | Formato | Descrição |
| :--- | :--- | :--- |
| [`Insomnia_SIAES_Collection.json`](./Insomnia_SIAES_Collection.json) | **Insomnia v4 (Nativo)** | Workspace e Collection nativa do Insomnia com 51 requisições, variáveis de ambiente e encadeamento automático de respostas (*Response Tag Chaining*). |
| [`Postman_SIAES_Collection.json`](./Postman_SIAES_Collection.json) | **Postman v2.1** | Compatível com o importador Postman do Insomnia, com scripts de validação de status e armazenamento de variáveis (`pm.environment.set`). |
| [`Postman_SIAES_Environment.json`](./Postman_SIAES_Environment.json) | **Postman Environment** | Ambiente com `baseUrl` (`http://localhost:5180`), credenciais de Admin e variáveis de contexto, ajustar conforme necessário. |

---

## 🚀 Como Importar no Insomnia

1. Abra o **Insomnia**.
2. Na tela de importação (ou clique no menu do Workspace / Projeto ➔ **Import**):
3. Arraste ou selecione o arquivo **[`Insomnia_SIAES_Collection.json`](./Insomnia_SIAES_Collection.json)**
4. O Insomnia reconhecerá o formato nativo e carregará o Workspace com as 8 pastas e as 51 requisições.

> [!TIP]
> Caso queira importar o ambiente separado ou usar o parser do Postman, você também pode arrastar o arquivo [`Postman_SIAES_Collection.json`](./Postman_SIAES_Collection.json) ou a especificação [`swagger.json`](./swagger.json), ambos 100% suportados nativamente pelo Insomnia.

---

## 🔄 Fluxos E2E Organizados nas Pastas

A collection segue a ordem cronológica do ciclo operacional da oficina:

```text
📁 01 - Autenticação & Usuários
    ├── 1.1 - Login Admin [Gera Token JWT]
    ├── 1.2 - Criar Usuário (Autocadastro)
    ├── 1.3 - Obter Usuário por ID
    ├── 1.4 - Listar Usuários (Paginado)
    ├── 1.5 - Alterar Senha do Usuário
    ├── 1.6 - Atualizar Usuário
    └── 1.7 - Deletar Usuário de Teste

📁 02 - Gestão de Clientes
    ├── 2.1 - Criar Novo Cliente (com CPF e Endereço)
    ├── 2.2 - Obter Cliente por ID
    ├── 2.3 - Listar Clientes (Paginado)
    └── 2.4 - Atualizar Dados do Cliente

📁 03 - Gestão de Veículos
    ├── 3.1 - Criar Veículo Vinculado ao Cliente
    ├── 3.2 - Obter Veículo por ID
    ├── 3.3 - Listar Veículos (Paginado)
    └── 3.4 - Atualizar Dados do Veículo

📁 04 - Catálogo de Serviços
    ├── 4.1 - Criar Serviço no Catálogo
    ├── 4.2 - Obter Serviço por ID
    ├── 4.3 - Listar Serviços (Paginado)
    └── 4.4 - Atualizar Serviço

📁 05 - Estoque de Peças
    ├── 5.1 - Cadastrar Peça no Estoque
    ├── 5.2 - Registrar Entrada de Estoque (Abastecer 20 un)
    ├── 5.3 - Obter Item de Estoque por ID (Valida Saldo)
    ├── 5.4 - Listar Itens de Estoque (Paginado)
    └── 5.5 - Atualizar Item de Estoque

📁 06 - Ordem de Serviço (Fluxo Principal)
    ├── 6.1 - Criar OS (Status: Recebida)
    ├── 6.2 - Obter Detalhes da OS (Recebida)
    ├── 6.3 - Iniciar Diagnóstico (Status: EmDiagnostico)
    ├── 6.4 - Adicionar Serviço à OS
    ├── 6.5 - Adicionar Peça com Reserva de Estoque
    ├── 6.6 - Gerar Orçamento (Status: AguardandoAprovacao)
    ├── 6.7 - Consulta Pública Status por ID (Sem Auth)
    ├── 6.8 - Consulta Pública Status por CPF (Sem Auth)
    ├── 6.9 - Aprovar Orçamento (Status: EmExecucao)
    ├── 6.10 - Finalizar Serviços (Status: Finalizada - Baixa Estoque)
    ├── 6.11 - Entregar Veículo (Status: Entregue)
    ├── 6.12 - Consultar OS Concluída (Histórico Completo)
    └── 6.13 - Métrica: Tempo Médio de Execução das OS

📁 07 - Ordem de Serviço (Fluxo Alternativo)
    ├── 7.1 - Criar 2ª OS
    ├── 7.2 - Iniciar Diagnóstico 2ª OS
    ├── 7.3 - Adicionar Peça com Reserva 2ª OS
    ├── 7.4 - Gerar Orçamento 2ª OS
    ├── 7.5 - Aprovar Orçamento 2ª OS (EmExecucao)
    ├── 7.6 - Solicitar Aprovação Adicional (Volta AguardandoAprovacao)
    ├── 7.7 - Rejeitar Orçamento (Status: Cancelada - Libera Estoque)
    └── 7.8 - Validar Status Cancelada

📁 08 - Limpeza e Soft Delete (Cleanup)
    ├── 8.1 - Deletar Peça de Estoque (Soft Delete)
    ├── 8.2 - Deletar Serviço do Catálogo (Soft Delete)
    ├── 8.3 - Deletar Veículo (Soft Delete)
    └── 8.4 - Deletar Cliente (Soft Delete)
```
