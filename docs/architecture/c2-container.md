# Diagrama de Contêineres (C2 do Modelo C4)

Este documento apresenta o **Diagrama de Contêineres (C2)** do sistema **SIAES (Sistema Integrado de Atendimento e Execução de Serviços)**, detalhando os contêineres de software de alto nível executáveis (aplicações, APIs e bancos de dados), seus protocolos de comunicação e como eles interagem entre si e com serviços externos.

O modelo segue fielmente a especificação definida no arquivo [`C4_ClientesVeiculos_Container_C2.drawio`](drawio/C4_ClientesVeiculos_Container_C2.drawio).

---

## 1. Visão Geral (Nível C2)

No modelo C4, o diagrama de **Contêineres (C2)** decompõe o sistema de software em seus blocos executáveis ou armazenamentos de dados independentes (cada um podendo ser implantado, escalado e executado separadamente):
- **Aplicações e APIs**: onde residem as regras e a interface programática.
- **Armazenamento de Dados**: bancos relacionais ou NoSQL para persistência durável.
- **Protocolos e Portas**: a tecnologia de transporte e integração (`HTTP / REST`, `SQL / TCP 5432`, `API / SMTP`).

---

## 2. Diagrama C2 — Contêineres do Sistema

```mermaid
flowchart TD
    %% Usuários
    Usuarios["Usuários da Oficina\n[Persons]\nAtendentes, mecânicos e administradores"]:::person

    %% Fronteira do Sistema SIAES
    subgraph BoundarySIAES ["Container SIAES [SIAES]"]
        API["SIAES.API\n[Container · ASP.NET Core / .NET]\nAPI responsável pelas operações do sistema,\nregras de aplicação e integrações."]:::container
        DB[("SIAES_DB\n[PostgreSQL: Entity Framework Core]\nPostgreSQL 16-ALPINE\nPersistência transacional e relacional")]:::db
    end

    %% Provedor Externo
    EmailService["Serviço de envio de e-mails\n[Software System]\nServiço externo utilizado pelo SIAES para\nenviar comunicações aos clientes da oficina"]:::external

    %% Relacionamentos e Protocolos
    Usuarios -->|"HTTP / REST\n(JSON, Bearer JWT)"| API
    API -->|"SQL via EF Core\n(TCP 5432 / Npgsql)"| DB
    API -->|"API / SMTP\n(HTTPS REST / Resend)"| EmailService

    %% Estilos visuais C4 (alinhados ao Draw.io)
    classDef person fill:#083F75,stroke:#06315C,color:#ffffff;
    classDef container fill:#1061B0,stroke:#0D5091,color:#ffffff;
    classDef db fill:#23A2D9,stroke:#0E7DAD,color:#ffffff;
    classDef external fill:#8C8496,stroke:#736782,color:#ffffff;
```

---

## 3. Catálogo de Contêineres e Tecnologias

### 3.1. Contêineres Internos

| Contêiner | Tipo / Tecnologia | Descrição e Responsabilidade |
| :--- | :--- | :--- |
| **`SIAES.API`** | `Container · ASP.NET Core / .NET 10` | Aplicação Web API backend monolítica modularizada em Clean Architecture. Expõe endpoints RESTful com versionamento (`v1`), gerencia autenticação JWT (RBAC), orquestra os casos de uso de negócio, valida dados e criptografa dados sensíveis de clientes. |
| **`SIAES_DB`** | `Container: PostgreSQL 16-ALPINE` | Banco de dados relacional principal da oficina. Armazena as entidades de Clientes, Veículos, Ordens de Serviço, Histórico imutável de transições, Itens de Estoque e Usuários, com suporte a índices únicos e integridade referencial ACID. |

### 3.2. Integrações e Comunicação

| Origem | Destino | Protocolo / Transporte | Descrição |
| :--- | :--- | :--- | :--- |
| **Usuários da Oficina** | **`SIAES.API`** | `HTTP / REST` | Requisições HTTP com payload JSON protegidas por tokens Bearer JWT via Swagger UI, cliente web ou requisições de terminal. |
| **`SIAES.API`** | **`SIAES_DB`** | `SQL via EF Core` | Conexão relacional via porta 5432 usando Npgsql e Entity Framework Core 10, com pool de conexões e migrações automáticas. |
| **`SIAES.API`** | **Serviço de e-mails** | `API / SMTP` | Chamadas HTTP/REST criptografadas (TLS 1.3) ao provedor de mensageria (Resend API) para notificações operacionais. |

---

## 4. Arquivo Draw.io Nativo

O diagrama original editável está disponível em:
- 📁 [`docs/architecture/drawio/C4_ClientesVeiculos_Container_C2.drawio`](drawio/C4_ClientesVeiculos_Container_C2.drawio)

