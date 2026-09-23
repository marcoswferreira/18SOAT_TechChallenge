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

<!-- TODO: Descrever o objetivo do projeto conforme o enunciado do Tech Challenge -->

Desenvolver a primeira versão (MVP) de um sistema back-end para gestão de oficinas mecânicas, contemplando...

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

### 3. Acessar a aplicação

<!-- TODO: Confirmar porta e URL -->

A API estará disponível em: `http://localhost:8000`



## 🧪 Instruções para Execução dos Testes

### Testes unitários

```bash
dotnet test tests/Domain.UnitTests
dotnet test tests/Application.UnitTests
```

### Testes de integração

```bash
dotnet test tests/IntegrationTests
```

### Executar todos os testes

```bash
dotnet test
```

<!-- TODO: Adicionar informações sobre cobertura de testes, se configurada -->

---

## 📖 Documentação da API (Swagger)

<!-- TODO: Confirmar URL do Swagger -->

Com a aplicação em execução, acesse a documentação interativa:

```
http://localhost:8000/swagger
```

<!-- TODO: Inserir screenshot da interface do Swagger -->

---

## 🗄️ Banco de Dados — Decisão e Justificativa

**Banco escolhido:** PostgreSQL 16

**Justificativa:**

<!-- TODO: Descrever os critérios e razões da escolha. Sugestões: -->

- ...
- ...
- ...

---

## 🔐 Autenticação e Credenciais de Demonstração

<!-- TODO: Descrever o mecanismo de autenticação utilizado (JWT, API Key, etc.) -->
<!-- TODO: Listar usuários/credenciais de demonstração para avaliação -->

| Perfil        | Usuário / E-mail       | Senha          |
|---------------|------------------------|----------------|
| Administrador | `admin@siaes.com`      | `SenhaAdmin1!` |
| Atendente     | `atendente@siaes.com`  | `SenhaAten1!`  |

> ⚠️ **Nota:** Estas credenciais são apenas para fins de demonstração e avaliação.

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