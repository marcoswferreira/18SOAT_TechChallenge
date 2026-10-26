# 🛠️ SIAES — Sistema Integrado de Atendimento e Execução de Serviços

![CI Pipeline](https://github.com/marcoswferreira/18SOAT_TechChallenge/actions/workflows/ci.yml/badge.svg)

> **FIAP — Pós-Graduação em Software Architecture · Turma 18SOAT**  
> **Tech Challenge — Fase 1**: MVP do back-end para gestão operacional e administrativa de oficinas mecânicas, aplicando **Domain-Driven Design (DDD)**, **Clean Architecture / Ports and Adapters**, segurança (LGPD/JWT) e práticas avançadas de qualidade de software.

---

## 📑 Índice

- [Objetivo do Projeto](#-objetivo-do-projeto)
- [Descrição da Solução](#-descrição-da-solução)
- [Arquitetura](#-arquitetura)
- [Tecnologias Utilizadas](#-tecnologias-utilizadas)
- [Pré-requisitos](#-pré-requisitos)
- [Instruções para Execução](#-instruções-para-execução)
- [Instruções para Execução dos Testes](#-instruções-para-execução-dos-testes)
- [Documentação da API (Swagger & Endpoints)](#-documentação-da-api-swagger--endpoints)
- [Banco de Dados — Decisão e Justificativa (ADR 001)](#-banco-de-dados--decisão-e-justificativa-adr-001)
- [Autenticação e Credenciais de Demonstração](#-autenticação-e-credenciais-de-demonstração)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Documentação DDD](#-documentação-ddd)
- [Vídeo da Entrega](#-vídeo-da-entrega)
- [Autores](#-autores)

---

## 🎯 Objetivo do Projeto

O objetivo principal deste projeto é desenvolver a primeira versão (**MVP**) do back-end para o **SIAES** (*Sistema Integrado de Atendimento e Execução de Serviços*), uma plataforma robusta e escalável projetada para modernizar a gestão de oficinas mecânicas.

A solução substitui controles manuais e desorganizados (baseados em blocos de papel e planilhas desconexas) por um sistema centralizado, auditável e seguro, fundamentado nas práticas de **Domain-Driven Design (DDD)** e arquitetura limpa.

### 🛠️ Problemas Resolvidos pela Solução
* **Perda de histórico e erros na triagem**: Falta de rastreabilidade de veículos e clientes, gerando retrabalho e diagnósticos imprecisos.
* **Furos no controle de estoque**: Peças retiradas sem baixa transacional ou duplicidade de uso por falta de controle de reserva.
* **Ineficiência no fluxo de orçamentos**: Morosidade no cálculo de peças e mão de obra, além de autorizações informais de clientes sem registro oficial.
* **Falta de transparência para o cliente**: Dificuldade em acompanhar em tempo real o status de execução e término da manutenção do veículo.

---

## 📝 Descrição da Solução

O **SIAES** orquestra ponta a ponta as rotinas operacionais de uma oficina mecânica, contemplando os seguintes módulos e fluxos de negócio:

### 1. Gestão de Clientes e Veículos (Conformidade LGPD)
* Cadastro completo de clientes com validação de **CPF ou CNPJ** por meio do Objeto de Valor (`Documento`).
* **Proteção de Dados Sensíveis**: o `Documento` do cliente e a `Placa` do veículo são gravados com **criptografia AES-256 determinística** (o que permite busca por igualdade e índice único) e possuem representação mascarada (ex.: `529.***.***-25`).
* Cadastro de veículos vinculados ao cliente contendo Placa (padrão antigo ou Mercosul), Marca, Modelo e Ano.

### 2. Catálogo de Serviços e Controle de Estoque
* **Catálogo de Serviços**: cadastro de tipos de serviço (ex.: troca de óleo e filtros, alinhamento e balanceamento). O valor do serviço é informado ao incluí-lo na OS.
* **Estoque Transacional com Reserva**: controle de saldo físico (`QuantidadeEmEstoque`) e saldo reservado (`QuantidadeReservada`), com concorrência otimista. Ao vincular uma peça a uma OS, o saldo disponível é imediatamente reservado para evitar uso duplicado.

### 3. Ciclo de Vida da Ordem de Serviço (OS)
A Ordem de Serviço é a raiz de agregação central do sistema, implementando uma máquina de estados finita estrita:
```mermaid
stateDiagram-v2
    [*] --> Recebida: Criação da OS
    Recebida --> EmDiagnostico: Iniciar Diagnóstico
    EmDiagnostico --> AguardandoAprovacao: Gerar Orçamento
    AguardandoAprovacao --> EmExecucao: Cliente Aprova
    AguardandoAprovacao --> Cancelada: Cliente Rejeita
    EmExecucao --> AguardandoAprovacao: Solicitar Aprovação Adicional
    EmExecucao --> Finalizada: Finalizar Serviço
    Finalizada --> Entregue: Registrar Entrega
    Entregue --> [*]
    Cancelada --> [*]
```

> A transição `EmExecucao → AguardandoAprovacao` (aprovação adicional) já existe no Domain, mas ainda não possui endpoint.

* **Geração Automática do Orçamento**: O valor total da OS é calculado a partir da soma dos serviços e peças adicionados.
* **Consistência de Estoque Integrada ao Ciclo de Vida**:
  * Ao **adicionar peças** à OS ➔ Reserva o saldo no estoque (`itemEstoque.Reservar()`).
  * Em caso de **rejeição do orçamento** ➔ Libera a reserva no estoque (`itemEstoque.LiberarReserva()`).
  * Na **finalização do serviço** ➔ Efetua a baixa definitiva do estoque físico e da reserva (`itemEstoque.BaixarReserva()`).

### 4. Fluxo de Notificações por E-mail (Integração Resend)
* Notificação automática ao cliente via **Resend**, com templates HTML responsivos, nos momentos críticos do atendimento:
  * **Orçamento Disponível para Aprovação**: E-mail detalhado com tabela discriminada de serviços, peças, valores unitários e valor total da OS, com instruções claras para autorização.
  * **Veículo Recebido**: Confirmação da entrada do veículo na oficina e início da triagem.
  * **Orçamento Aprovado**: Confirmação do início da execução dos serviços pela equipe técnica.
  * **Orçamento Rejeitado / Cancelamento**: Confirmação do cancelamento e liberação do veículo.
  * **Veículo Pronto para Retirada**: Notificação de conclusão dos serviços com horário e local de retirada.
  * **Veículo Entregue**: Confirmação de encerramento do atendimento e agradecimento.
* **Resiliência**: o envio ocorre **após** a persistência da operação e é aguardado dentro da requisição (sem fila). Cliente sem e-mail cadastrado é ignorado (log informativo); falhas do provedor são capturadas e registradas em log de erro, sem interromper nem desfazer a operação de negócio já gravada.

### 5. Consulta Pública de Status
* Endpoints públicos que permitem ao cliente consultar o status atual da sua OS em tempo real informando o **ID da Ordem de Serviço** ou o **CPF/CNPJ**, garantindo transparência sem exigir credenciais administrativas.

---

## 🏛️ Arquitetura

O projeto adota os padrões **Clean Architecture** e **Ports and Adapters (Hexagonal)**, alinhados aos princípios táticos e estratégicos do **Domain-Driven Design (DDD)**:

```mermaid
graph TD
    subgraph Presentation ["Presentation (SIAES.API)"]
        Controllers["Controllers REST (API Versionada)"]
        Middlewares["Middlewares (ExceptionHandler, Auth JWT)"]
    end

    subgraph Application ["Application Layer"]
        UseCases["Casos de Uso (Use Cases)"]
        DTOs["Data Transfer Objects (DTOs)"]
        Validators["Validators de entrada"]
    end

    subgraph Domain ["Domain Layer (Core)"]
        Entities["Entidades Ricas (OrdemServico, Cliente, ItemEstoque, Veiculo)"]
        ValueObjects["Value Objects (Documento, Placa, Endereco, ItemPeca, ItemServico)"]
        DomainInterfaces["Interfaces de Repositórios & Serviços"]
    end

    subgraph Infrastructure ["Infrastructure Layer"]
        EFCore["Entity Framework Core 10 (PostgreSQL)"]
        Repositories["Implementações de Repositório (BaseRepository)"]
        NotificationService["Serviço de Notificação (HTML Templates)"]
        ResendAdapter["Adaptador Resend (E-mail Transacional)"]
        Security["Segurança (AES-256, PasswordHasher)"]
    end

    Presentation --> Application
    Presentation --> Infrastructure
    Application --> Domain
    Infrastructure --> Domain
    Infrastructure --> Application
```

### Divisão de Responsabilidades

| Camada | Projeto | Responsabilidades |
| :--- | :--- | :--- |
| **Domain** | `src/Domain` | Núcleo do sistema. Contém entidades com encapsulamento de invariantes (`OrdemServico`, `Cliente`, `ItemEstoque`, `Veiculo`, `Servico`, `User`), Objetos de Valor imutáveis (`Documento`, `Placa`, `Endereco`, `ItemPeca`, `ItemServico`, `RegistroStatus`), exceções de domínio e as **portas** (`IOrdemServicoRepository`, `IResendEmailService`, `IOrdemServicoNotificationService`, etc.). Não referencia nenhum outro projeto da solução; o único pacote externo é o EF Core, usado nos helpers de paginação (débito técnico registrado no roadmap). |
| **Application** | `src/Application` | Orquestração dos fluxos de negócio através de **Use Cases** atômicos (ex.: `CreateOrdemServicoUseCase`, `GerarOrcamentoUseCase`, `FinalizarOrdemServicoUseCase`), DTOs de entrada/saída e validações. Não conhece detalhes de banco de dados ou provedores de mensageria. |
| **Infrastructure** | `src/Infrastructure` | Implementações técnicas concretas: contexto do banco de dados (`ApplicationDbContext`), mapeamento fluente do EF Core, migrações, repositórios com `AsSplitQuery()`, serviço de notificação com geração de HTML, cliente do **Resend**, criptografia determinística AES-256 e seeders. |
| **Presentation** | `src/SIAES.API` | Ponto de entrada HTTP RESTful. Configuração da injeção de dependência (IoC), versionamento de API (`v1.0`), autenticação JWT com rotação de tokens, documentação interativa via **Swagger/OpenAPI** e tratamento global de exceções. |

---

## 🚀 Tecnologias Utilizadas

- **Plataforma & Linguagem**: [.NET 10](https://dotnet.microsoft.com/) / C# 14
- **Framework Web**: [ASP.NET Core Web API](https://learn.microsoft.com/aspnet/core/)
- **ORM & Persistência**: [Entity Framework Core 10](https://learn.microsoft.com/ef/core/) com [Npgsql.EntityFrameworkCore.PostgreSQL](https://www.npgsql.org/efcore/)
- **Banco de Dados Relacional**: [PostgreSQL 16-alpine](https://www.postgresql.org/)
- **Envio de E-mails Transacionais**: [Resend .NET SDK](https://resend.com/)
- **Segurança & Criptografia**:
  - `PasswordHasher` do **ASP.NET Core Identity** (PBKDF2 com salt) para hash de senhas
  - `System.Security.Cryptography` com **AES-256 determinístico** para proteger `Documento` e `Placa` em repouso (permite busca por igualdade e índice único)
  - Autenticação e Autorização via **JWT (JSON Web Token)** com *refresh token* (cookie `HttpOnly`) e perfis por `Roles`
- **Conteinerização**: [Docker](https://www.docker.com/) e [Docker Compose](https://docs.docker.com/compose/)
- **Documentação da API**: [Swagger / Swashbuckle](https://swagger.io/) com suporte a OpenAPI e Versionamento
- **Testes Automatizados**:
  - [xUnit](https://xunit.net/) — Framework de testes
  - [NSubstitute](https://nsubstitute.github.io/) — Biblioteca para mocks e stubs
  - [Coverlet](https://github.com/coverlet-coverage/coverlet) — Coleta de cobertura de código
- **Qualidade & CI/CD**:
  - [GitHub Actions](https://github.com/features/actions) — Pipeline automatizado de compilação, testes e build Docker
  - [SonarCloud](https://sonarcloud.io/) — Análise estática de código, detecção de vulnerabilidades e controle de duplicação

---

## ✅ Pré-requisitos

Para executar e testar a aplicação em ambiente local:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (com Docker Compose ativo)
- [Git](https://git-scm.com/)
- *(Opcional)* Token de API do [Resend](https://resend.com/) caso queira disparar e-mails reais para caixas de entrada externas durante testes manuais (a aplicação já possui fallback seguro e funcional para desenvolvimento local).

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

> ℹ️ **Migrações e Carga Inicial Automáticas:**  
> Em ambiente `Development`, a API aplica as migrações pendentes no PostgreSQL e executa o *seed* (usuários de demonstração, clientes, veículos, itens de estoque e serviços padrão) ao iniciar. O `docker-compose.override.yml` — carregado automaticamente pelo `docker-compose up` — já define `ASPNETCORE_ENVIRONMENT=Development`, as portas (`8082` para a API e `5432` para o PostgreSQL) e a *connection string*. Fora de `Development`, migrações, *seed* e Swagger não são executados.

### 3. Acessar a aplicação

- **API Base URL**: `http://localhost:8082`
- **Swagger UI**: `http://localhost:8082/swagger`

---

## 🧪 Instruções para Execução dos Testes

O projeto conta com **406 testes automatizados** cobrindo isoladamente as regras do domínio, os casos de uso da aplicação, integrações com segurança, arquitetura e **98,6% de cobertura de código em testes de integração nos principais controllers e endpoints HTTP**, mantendo **100% de taxa de sucesso**.

```
Passed! - Failed: 0, Passed: 186, Total: 186 - Domain.UnitTests.dll
Passed! - Failed: 0, Passed: 121, Total: 121 - Application.UnitTests.dll
Passed! - Failed: 0, Passed:  99, Total:  99 - IntegrationTests.dll
Total: 406 testes passando
```

### Executar todos os testes da solução:

```bash
dotnet test SIAES.slnx
```

### Executar por projeto:

```bash
# Testes Unitários de Domínio (Entidades, Value Objects, Validações)
dotnet test tests/Domain.UnitTests

# Testes Unitários de Aplicação (Use Cases, DTOs, Orquestrações)
dotnet test tests/Application.UnitTests

# Testes de Integração (Criptografia, Serviços de Infraestrutura)
dotnet test tests/IntegrationTests
```

---

## 📖 Documentação da API (Swagger & Endpoints)

Com a aplicação rodando, acesse a documentação interativa:
```
http://localhost:8082/swagger
```

### Principais Grupos de Endpoints

| Módulo | Método | Rota | Descrição |
| :--- | :--- | :--- | :--- |
| **Auth** | `POST` | `/api/v1/Auth/login` | Autenticação com e-mail/senha e emissão de tokens |
| | `POST` | `/api/v1/Auth/refresh-token` | Rotação e renovação do Access Token |
| | `POST` | `/api/v1/Auth/logout` | Encerra a sessão revogando o *refresh token* ativo |
| **Ordens de Serviço** | `GET` | `/api/v1/ordens-servico/{id}/status` | **Consulta pública** do status da OS por ID |
| | `GET` | `/api/v1/ordens-servico/documento/{doc}/status`| **Consulta pública** das OS de um cliente por CPF/CNPJ |
| | `POST` | `/api/v1/ordens-servico` | Criação de uma nova OS (Status inicial: `Recebida`) |
| | `GET` | `/api/v1/ordens-servico/{id}` | Obtenção detalhada da OS com peças, serviços e histórico |
| | `GET` | `/api/v1/ordens-servico` | Listagem paginada de Ordens de Serviço |
| | `POST` | `/api/v1/ordens-servico/{id}/diagnostico` | Início do diagnóstico técnico (`EmDiagnostico`) |
| | `POST` | `/api/v1/ordens-servico/{id}/pecas` | Adiciona peça à OS e **reserva saldo no estoque** |
| | `POST` | `/api/v1/ordens-servico/{id}/servicos` | Adiciona serviço/mão de obra à OS |
| | `POST` | `/api/v1/ordens-servico/{id}/orcamento` | Gera orçamento e **envia e-mail ao cliente** (`AguardandoAprovacao`) |
| | `POST` | `/api/v1/ordens-servico/{id}/aprovar` | Registra aprovação do cliente e inicia execução (`EmExecucao`) |
| | `POST` | `/api/v1/ordens-servico/{id}/solicitar-aprovacao-adicional` | Solicita aprovação adicional de orçamento durante a execução (`AguardandoAprovacao`) |
| | `POST` | `/api/v1/ordens-servico/{id}/rejeitar` | Rejeita orçamento, cancela OS e **libera reservas de estoque** |
| | `POST` | `/api/v1/ordens-servico/{id}/finalizar` | Finaliza serviços, **baixa estoque físico** e notifica cliente |
| | `POST` | `/api/v1/ordens-servico/{id}/entregar` | Registra entrega do veículo e encerra OS (`Entregue`) |
| **Clientes** | `CRUD` | `/api/v1/Clientes` | Gestão de clientes com validação e criptografia de documento |
| **Veículos** | `CRUD` | `/api/v1/Veiculos` | Gestão de veículos e vínculo com clientes |
| **Estoque** | `CRUD` | `/api/v1/ItensEstoque` | Gestão de itens de estoque (`POST /{id}/entradas` registra entrada) |
| **Serviços** | `CRUD` | `/api/v1/Servicos` | Catálogo de tipos de serviço |
| **Usuários** | `CRUD` | `/api/v1/Users` | Gestão de usuários (`PUT /{id}/password` troca a senha) |

> O roteamento do ASP.NET Core não diferencia maiúsculas de minúsculas (`/api/v1/clientes` também funciona). As regras de acesso de cada recurso estão em [Regras de Acesso Atuais](#regras-de-acesso-atuais).

---

## 🗄️ Banco de Dados — Decisão e Justificativa (ADR 001)

Para a camada de persistência do **SIAES**, foi adotado o **PostgreSQL 16** integrado ao **Entity Framework Core 10**.

### Resumo da Justificativa Técnica:
* **Garantias ACID e Consistência de Estoque**: Operações de reserva e baixa de peças exigem atomicidade e isolamento para evitar inconsistências sob concorrência.
* **Mapeamento Natural de Agregados do DDD**: O modelo relacional suporta perfeitamente as fronteiras dos agregados (`OrdemServico`, `Cliente`, `ItemEstoque`, `Veiculo`) com integridade referencial estrita e otimização de consultas com `AsSplitQuery()`.
* **Concorrência no Estoque**: `ItemEstoque` usa *row version* (`xmin` do PostgreSQL) como controle de concorrência otimista, evitando que duas OS reservem o mesmo saldo simultaneamente; a integração com .NET é feita pelo driver `Npgsql`.

📄 **Documentação de Arquitetura**: Consulte a [ADR 001 — Seleção do Banco de Dados Relacional](docs/adrs/adr-001-escolha-do-banco-de-dados.md) para análise detalhada de trade-offs.

---

## 🔐 Autenticação e Credenciais de Demonstração

A API utiliza autenticação JWT com controle de acesso por perfis (`Roles`): `Admin`, `Manager`, `Operacional` e `User`.

### Credenciais Pré-cadastradas (Seeded automaticamente):

| Ator | Usuário / E-mail | Senha | Role |
| :--- | :--- | :--- | :--- |
| **Administrador** | `admin@siaes.com` | `SenhaAdmin1!` | `Admin` |
| **Gerente** | `manager@siaes.com` | `SenhaManager1!` | `Manager` |
| **Atendente** | `atendente@siaes.com` | `SenhaAten1!` | `Operacional` |
| **Mecânico** | `mecanico@siaes.com` | `SenhaMecanico1!` | `Operacional` |
| **Usuário** | `usuario@siaes.com` | `SenhaUsuario1!` | `User` |

> `Atendente` e `Mecânico` são papéis de negócio distintos que compartilham o perfil de acesso `Operacional` (ver [Linguagem Ubíqua](docs/ddd/linguagem-ubiqua.md)).

### Como Autenticar no Swagger:
1. Acesse `http://localhost:8082/swagger`.
2. Execute a requisição `POST /api/v1/auth/login` informando um dos e-mails e senhas acima.
3. Copie o `accessToken` retornado no corpo da resposta.
4. Clique no botão **Authorize** (cadeado no topo direito do Swagger).
5. Digite `<seu_access_token>` e confirme.
6. As requisições aos endpoints protegidos serão autorizadas.

> ℹ️ O *access token* expira em 15 minutos e o *refresh token* em 7 dias. O refresh token é entregue em cookie `HttpOnly`; use `POST /api/v1/Auth/refresh-token` para renová-lo.

### Regras de Acesso Atuais

| Recurso | Quem acessa |
| :--- | :--- |
| `Auth` (login, refresh-token, logout) | Anônimo |
| `Users` | `POST` anônimo (autocadastro com perfil `User` padrão); listar e excluir: `Admin`; demais operações: autenticado |
| `Clientes` e `Veiculos` | Autenticado; excluir: `Admin` |
| `ItensEstoque` | Leitura: autenticado; criar, alterar e registrar entrada: `Admin`, `Manager`, `Operacional`; excluir: `Admin`, `Manager` |
| `Servicos` | Qualquer usuário autenticado |
| `OrdensServico` | Consultas públicas de status: anônimo; demais operações: `Admin`, `Manager`, `Operacional` |

> ℹ️ As principais pendências de segurança identificadas foram saneadas (proteção de rotas da OS e blindagem do payload de autocadastro contra escalação de privilégios). Demais débitos técnicos e melhorias estão listados no *Roadmap Técnico* de [`docs/ddd/architecture.md`](docs/ddd/architecture.md#8-roadmap-técnico).

---

## 📁 Estrutura do Projeto

```
├── .github/
│   └── workflows/
│       └── ci.yml                          # Pipeline CI (Build, Tests, SonarCloud, Docker)
├── docs/
│   ├── adrs/
│   │   └── adr-001-escolha-do-banco-de-dados.md # Decisão de Arquitetura (PostgreSQL)
│   ├── ddd/
│   │   ├── architecture.md                 # Arquitetura e padrões técnicos
│   │   ├── domain-storytelling.md          # Narrativas dos fluxos de negócio As-Is
│   │   ├── event-storming.md               # Workshop de eventos de domínio
│   │   └── linguagem-ubiqua.md             # Dicionário de termos ubíquos
│   └── images/                             # Diagramas visuais dos fluxos
├── src/
│   ├── Domain/                             # Núcleo da Aplicação (Core - 100% puro sem dependência de EF Core)
│   │   ├── Common/                         # Entidades base, exceções de domínio e paginação
│   │   ├── Constants/                      # Roles (Admin, Manager, Operacional, User)
│   │   ├── Entities/                       # OrdemServico, Cliente, Veiculo, ItemEstoque, etc.
│   │   ├── Enums/                          # StatusOrdemServico, UnidadeMedida, TipoDocumento
│   │   ├── Interfaces/                     # Contratos de Repositórios e Serviços (Portas)
│   │   └── ValueObjects/                   # Documento, Placa, Endereco, ItemPeca, ItemServico
│   ├── Application/                        # Orquestração de Negócio
│   │   ├── Dto/                            # Tipos de resultado (Result e ResultGeneric)
│   │   ├── Extensions/                     # Injeção de dependência dos Use Cases
│   │   └── UseCases/                       # Casos de Uso organizados por domínio
│   │       ├── Auth/                       # Login, Refresh e Revogação de tokens
│   │       ├── Clientes/                   # CRUD de Clientes
│   │       ├── ItensEstoque/               # Gestão de Estoque e Entradas
│   │       ├── OrdensServico/              # Ciclo de vida completo da OS e consultas
│   │       ├── Servicos/                   # Catálogo de Serviços
│   │       ├── Users/                      # Gestão de Usuários
│   │       └── Veiculos/                   # CRUD de Veículos
│   ├── Infrastructure/                     # Adaptadores de Entrada/Saída
│   │   ├── Database/                       # DbContext, Migrations, Repositories e Paginate
│   │   ├── Extensions/                     # Configuração de Infraestrutura e Resend
│   │   ├── Security/                       # Criptografia AES-256, Hash de senha e UserContext
│   │   ├── Services/                       # ResendEmailService, OrdemServicoNotificationService e TokenService
│   │   └── Settings/                       # Modelos de configuração fortemente tipados
│   └── SIAES.API/                          # Camada de Apresentação
│       ├── Controllers/                    # Controllers RESTful com versionamento
│       ├── Extensions/                     # Versionamento de API e Swagger
│       └── Program.cs                      # Bootstrapping e configuração do pipeline HTTP
├── tests/
│   ├── Domain.UnitTests/                   # 186 testes de regras e invariantes de domínio
│   ├── Application.UnitTests/              # 121 testes unitários dos Casos de Uso
│   └── IntegrationTests/                   # 99 testes de integração (endpoints REST com WebApplicationFactory, segurança, notificações e arquitetura)
├── docker-compose.yml                      # Orquestração base dos containers (API + PostgreSQL)
├── docker-compose.override.yml             # Ajustes locais: portas, ambiente Development e connection string
├── SIAES.slnx                              # Arquivo de solução moderno .NET
└── README.md                               # Documentação principal do projeto
```

---

## 📐 Documentação DDD e Arquitetura (C4 Model)

A documentação detalhada de **Domain-Driven Design** e **Arquitetura (C4 Model)** desenvolvida pelo grupo encontra-se em [`docs/`](docs/):

| Documento | Nível / Assunto | Descrição |
| :--- | :--- | :--- |
| **[C1 — Contexto do Sistema](docs/architecture/c1-contexto.md)** | **C4 Nível 1** | Diagrama de Contexto: escopo do sistema SIAES, personas e serviço de e-mails |
| **[C2 — Contêineres do Sistema](docs/architecture/c2-container.md)** | **C4 Nível 2** | Diagrama de Contêineres: `SIAES.API`, `SIAES_DB` e integrações REST/SQL/SMTP |
| **[C3 — Cliente e Veículo](docs/architecture/c3-cliente-veiculo.md)** | **C4 Nível 3** | Diagrama de Componentes por camadas Clean Architecture, validações e LGPD |
| **[C3 — Ordem de Serviço](docs/architecture/c3-ordem-servico.md)** | **C4 Nível 3** | Diagrama C3 da Ordem de Serviço, catálogo de componentes e fluxos |
| **[C3 — Gerenciamento de Estado](docs/architecture/c3-gerenciamento-estado.md)** | **C4 Nível 3** | Diagrama C3 da Máquina de Estados da OS, regras de transição e auditoria |
| **[Arquitetura da Solução](docs/ddd/architecture.md)** | Visão Geral | Decisões arquiteturais, camadas, catálogo de casos de uso e roadmap |
| **[Linguagem Ubíqua](docs/ddd/linguagem-ubiqua.md)** | DDD | Vocabulário padronizado compartilhado entre especialistas de negócio e devs |
| **[Domain Storytelling](docs/ddd/domain-storytelling.md)** | DDD | Narrativas e diagramas dos fluxos operacionais da oficina (*Cenários 1, 2 e 3*) |
| **[Event Storming](docs/ddd/event-storming.md)** | DDD | Mapeamento de eventos de domínio, comandos e reações do sistema |
| **[ADR 001 — Banco de Dados](docs/adrs/adr-001-escolha-do-banco-de-dados.md)** | ADR | Análise comparativa e justificativa da escolha do PostgreSQL |

> 📁 Os arquivos originais editáveis do Draw.io estão disponíveis na pasta [`docs/architecture/drawio/`](docs/architecture/drawio/).

---

## 🎬 Vídeo da Entrega

O vídeo demonstrativo com a explicação da arquitetura, fluxo de execução, testes automatizados e funcionamento prático do MVP está disponível no YouTube:

📹 **[Assistir ao vídeo da entrega no YouTube](https://youtube.com/todo)** *(preencher com o link do vídeo final)*

---

## 👥 Autores

| Nome | RM |
| :--- | :--- |
| Alexandre Martins Placido | RMXXXXXX |
| Diego Estefan Conceição de Pádua | RMXXXXXX |
| Jayr Rufino de Almeida Junior | RMXXXXXX |
| Marcos Welington Ferreira | RM378531 |
| Milena Vargas Leonardi | RMXXXXXX |

---

<p align="center">
  <strong>FIAP — Pós-Graduação em Software Architecture</strong><br>
  Tech Challenge · Fase 1 · Turma 18SOAT · 2026
</p>