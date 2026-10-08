# 🛠️ SIAES — Sistema Integrado de Atendimento e Serviços (Fase 1)

![CI Pipeline](https://github.com/marcoswferreira/18SOAT_TechChallenge/actions/workflows/ci.yml/badge.svg)

> **FIAP — Pós-Graduação em Software Architecture · Turma 18SOAT**
>
> Tech Challenge — Fase 1: MVP do back-end para gestão de oficinas mecânicas, aplicando Domain-Driven Design (DDD), arquitetura em camadas e práticas de Qualidade de Software.

---

## 📑 Índice

- [Objetivo do Projeto](#-objetivo-do-projeto)
- [Descrição da Solução](#-descrição-da-solução)
- [Arquitetura](#-arquitetura)
- [Tecnologias Utilizadas](#-tecnologias-utilizadas)
- [Pré-requisitos](#-pré-requisitos)
- [Instruções para Execução](#-instruções-para-execução)
- [Instruções para Execução dos Testes](#-instruções-para-execução-dos-testes)
- [Documentação da API (Swagger)](#-documentação-da-api-swagger)
- [Banco de Dados — Decisão e Justificativa](#-banco-de-dados--decisão-e-justificativa)
- [Autenticação e Credenciais de Demonstração](#-autenticação-e-credenciais-de-demonstração)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Documentação DDD](#-documentação-ddd)
- [Vídeo da Entrega](#-vídeo-da-entrega)
- [Autores](#-autores)

---

## 🎯 Objetivo do Projeto

O objetivo principal deste projeto é **desenvolver a primeira versão (MVP) do back-end** para o **SIAES** (*Sistema Integrado de Atendimento e Execução de Serviços*), uma plataforma destinada à gestão operacional e administrativa de oficinas mecânicas.

A solução visa substituir processos manuais e desorganizados (baseados em planilhas e anotações) por um sistema centralizado, seguro e escalável, aplicando os princípios do **Domain-Driven Design (DDD)**.

### 🛠️ Problemas Resolvidos pela Solução
* **Erros na priorização de atendimentos e perda de histórico** de clientes e veículos.
* **Falhas no controle de estoque** de peças e insumos.
* **Ineficiência na elaboração, envio e autorização de orçamentos**.
* **Falta de transparência e acompanhamento do status** do serviço em tempo real pelo cliente.

---

### 🚀 Escopo do MVP (Funcionalidades Principais)

1. **Gestão e Ciclo de Vida da Ordem de Serviço (OS)**:
   * Identificação do cliente via **CPF ou CNPJ** e cadastro completo de veículos (placa, marca, modelo e ano).
   * Inclusão de serviços solicitados, peças e insumos.
   * **Cálculo e geração automática do orçamento** para envio e aprovação/rejeição pelo cliente.
   * Controle automatizado dos status da OS (`Recebida` ➔ `Em diagnóstico` ➔ `Aguardando aprovação` ➔ `Em execução` ➔ `Finalizada` ➔ `Entregue`).
   * **API de consulta pública** para acompanhamento em tempo real pelo cliente.

2. **Gestão Administrativa e Operacional**:
   * CRUDs completos para **Clientes, Veículos, Serviços e Peças/Insumos**.
   * **Controle de estoque** com reserva e baixa transacional de peças.
   * **Monitoramento do tempo médio** de execução dos serviços para otimização da oficina.

3. **Arquitetura, Segurança e Qualidade do Código**:
   * Arquitetura de **Back-end Monolítico em Camadas** isolando as regras do Domínio.
   * Proteção das APIs administrativas por **autenticação JWT** e validação rigorosa de dados sensíveis.
   * Cobertura mínima de **80% em testes automatizados** (unitários e de integração) nos domínios críticos.
   * Ambiente totalmente conteinerizado via **Dockerfile** e **Docker Compose** para execução local simplificada.

---
## 📝 Descrição da Solução

<!-- TODO: Resumir a solução implementada, fluxos principais e escopo do MVP -->

...

---

## 🏛️ Arquitetura

<!-- TODO: Detalhar a arquitetura adotada. Incluir diagrama se possível -->

O projeto segue uma **arquitetura em camadas** (Layered / Clean Architecture), organizada nos seguintes níveis:

| Camada            | Projeto               | Responsabilidade                                     |
|-------------------|-----------------------|------------------------------------------------------|
| **Domain**        | `Domain`              | Entidades, Value Objects, interfaces de repositório   |
| **Application**   | `Application`         | Casos de uso, DTOs, validações de negócio             |
| **Infrastructure**| `Infrastructure`      | Persistência (EF Core + PostgreSQL), Segurança|
| **Presentation**  | `SIAES.API`           | Controllers, middlewares, configuração da API         |

<!-- TODO: Inserir diagrama de arquitetura (ex.: imagem em docs/images/) -->

---

## 🚀 Tecnologias Utilizadas

<!-- TODO: Atualizar conforme novas dependências forem adicionadas -->

- [.NET 10](https://dotnet.microsoft.com/) — SDK e runtime
- [ASP.NET Core](https://learn.microsoft.com/aspnet/core/) — Framework Web API
- [Entity Framework Core](https://learn.microsoft.com/ef/core/) — ORM
- [PostgreSQL 16](https://www.postgresql.org/) — Banco de dados relacional
- [Docker / Docker Compose](https://www.docker.com/) — Containerização
- [Swagger / Swashbuckle](https://swagger.io/) — Documentação interativa da API
- [xUnit](https://xunit.net/) — Framework de testes
- [GitHub Actions](https://github.com/features/actions) — CI/CD

---

## ✅ Pré-requisitos

Antes de executar o projeto, certifique-se de ter instalado:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (com Docker Compose)
- [Git](https://git-scm.com/)

<!-- TODO: Adicionar outros pré-requisitos, se necessário -->

---

## ▶️ Instruções para Execução

### 1. Clonar o repositório

```bash
git clone https://github.com/marcoswferreira/18SOAT_TechChallenge.git
cd 18SOAT_TechChallenge
```

### 2. Subir os containers com Docker Compose

```bash
docker-compose up -d --build
```

> ℹ️ **Carga Inicial e Banco de Dados Automático:**
> Na inicialização da API, o Entity Framework Core verifica se as tabelas do PostgreSQL foram criadas. Caso contrário, ele aplica as migrações/cria o esquema e popula automaticamente os usuários padrão de demonstração descritos no README.

### 3. Acessar a aplicação

A API estará disponível em: `http://localhost:8082`

---

## 🧪 Instruções para Execução dos Testes

### Testes unitários do Domínio

```bash
dotnet test tests/Domain.UnitTests
```

### Testes unitários da Aplicação

```bash
dotnet test tests/Application.UnitTests
```

### Testes de integração

```bash
dotnet test tests/IntegrationTests
```

### Executar todos os testes da solução

```bash
dotnet test SIAES.slnx
```

---

## 📖 Documentação da API (Swagger)

Com a aplicação em execução, acesse a documentação interativa para testar os endpoints:

```
http://localhost:8082/swagger
```

---

## 🗄️ Banco de Dados — Decisão e Justificativa

Para a camada de persistência e suporte às regras de negócio do **SIAES**, foi adotado o **PostgreSQL 16** (utilizando a imagem oficial `postgres:16-alpine`), integrado ao **Entity Framework Core 10** na camada de Infraestrutura.

### 📋 Resumo da Justificativa Técnica

* **Garantias ACID e Consistência de Estoque:** Ações de aprovação de orçamento e reserva/baixa de peças exigem transações atômicas e isoladas para evitar concorrência ou vendas sem saldo.
* **Mapeamento de Agregados do DDD:** Estruturação relacional ideal para representar as fronteiras de consistência dos Agregados (`OrdemDeServico`, `Cliente`, `Veiculo`, `Estoque`) com integridade referencial forte.
* **Recursos Híbridos (`JSONB`):** Suporte nativo a dados semi-estruturados, permitindo salvar *snapshots* de orçamentos e históricos de diagnósticos sem necessitar de um banco NoSQL secundário.
* **Integração com o Ecossistema .NET:** Conectividade madura via driver `Npgsql`, com suporte a *EF Core Migrations* aplicadas automaticamente na inicialização do container.
* **Execução Local Simplificada:** Conteinerização leve com baixo consumo de memória RAM/CPU e suporte a *healthchecks* nativos (`pg_isready`) no Docker Compose.

📄 **Documentação Técnica de Arquitetura (ADR)**  
Para conferir a análise detalhada de *trade-offs*, mitigações e os motivos do descarte das alternativas (como MongoDB e SQL Server), acesse a **[ADR 001 — Seleção do Banco de Dados Relacional](docs/adrs/adr-001-escolha-do-banco-de-dados.md)**.


---

## 🔐 Autenticação e Credenciais de Demonstração

A API possui autenticação JWT com rotação de **Refresh Token** via HTTP-Only Cookies e Headers.

### Credenciais pré-cadastradas (Seeded automaticamente):

| Perfil        | Usuário / E-mail       | Senha          | Role        |
|---------------|------------------------|----------------|-------------|
| Administrador | `admin@siaes.com`      | `SenhaAdmin1!` | `Admin`     |
| Atendente     | `atendente@siaes.com`  | `SenhaAten1!`  | `Operacional` |
| Mecanico     | `mecanico@siaes.com`   | `SenhaMec1!`   | `Mecanico`  |
| Usuario     | `usuario@siaes.com`  | `SenhaUsuario1!`  | `User` |

### Como Autenticar no Swagger:
1. Acesse `http://localhost:8082/swagger`.
2. Faça uma requisição em `POST /api/v1/auth/login` informando um dos e-mails e senhas acima.
3. Copie o `accessToken` retornado no corpo da resposta.
4. Clique no botão **Authorize** no canto superior do Swagger.
5. Digite `<seu_access_token>` e confirme.
6. Agora você pode executar requisições para os endpoints protegidos (`/api/v1/users`, etc.).

---

## 📁 Estrutura do Projeto

<!-- TODO: Atualizar conforme o projeto evoluir -->

```
├── .github/
│   └── workflows/
│       └── ci.yml                  # Pipeline de CI (GitHub Actions)
├── docs/
│   ├── ddd/
│   │   ├── architecture.md         # Documentação da arquitetura
│   │   ├── domain-storytelling.md  # Domain Storytelling
│   │   ├── event-storming.md       # Event Storming
│   │   └── linguagem-ubiqua.md     # Linguagem Ubíqua
│   └── images/                     # Diagramas e imagens
├── src/
│   ├── Domain/                     # Camada de Domínio
│   ├── Application/                # Camada de Aplicação
│   ├── Infrastructure/             # Camada de Infraestrutura
│   └── SIAES.API/                  # Camada de Apresentação (API)
├── tests/
│   ├── Domain.UnitTests/           # Testes unitários do domínio
│   ├── Application.UnitTests/      # Testes unitários da aplicação
│   └── IntegrationTests/           # Testes de integração
├── docker-compose.yml              # Orquestração dos containers
├── SIAES.slnx                      # Solution file
└── README.md
```

---

## 📐 Documentação DDD

A documentação completa de Domain-Driven Design está disponível na pasta [`docs/ddd/`](docs/ddd/):

| Documento | Descrição |
|-----------|-----------|
| [Linguagem Ubíqua](docs/ddd/linguagem-ubiqua.md) | Glossário dos termos do domínio |
| [Domain Storytelling](docs/ddd/domain-storytelling.md) | Narrativas dos fluxos de negócio |
| [Event Storming](docs/ddd/event-storming.md) | Mapeamento de eventos de domínio |
| [Arquitetura](docs/ddd/architecture.md) | Decisões e diagramas de arquitetura |

<!-- TODO: Adicionar links para artefatos adicionais do DDD (ex.: Mapa de Contexto, diagramas) -->

---

## 🎬 Vídeo da Entrega

<!-- TODO: Inserir link do vídeo demonstrando o funcionamento do projeto -->

📹 [Assistir vídeo da entrega no YouTube](https://youtube.com/todo)

---

## 👥 Autores

| Nome | RM |
|------|----|
| Alexandre Martins Placido | RMXXXXXX |
| Diego Estefan Conceição de Pádua| RMXXXXXX |
| Jayr Rufino de Almeida Junior| RMXXXXXX |
| Marcos Welington Ferreira | RM378531 |
| Milena Vargas Leonardi| RMXXXXXX |

---

<p align="center">
  <strong>FIAP — Pós-Graduação em Software Architecture</strong><br>
  Tech Challenge · Fase 1 · 2026
</p>