# Diagrama de Componentes (C3 do Modelo C4) — Cliente e Veículo

Este documento descreve a arquitetura do subsistema de **Cliente e Veículo** no sistema **SIAES**, apresentando o **Diagrama de Componentes (C3 do modelo C4)** que detalha as camadas de software da API Backend, a organização dos casos de uso, as regras do domínio, a persistência e a conformidade com a LGPD.

O modelo segue fielmente a estrutura e a representação de camadas definida no arquivo [`C4_ClientesVeiculos_Component_C3.drawio`](drawio/C4_ClientesVeiculos_Component_C3.drawio).

---

## 1. Visão Geral (Nível C3)

No modelo C4, o diagrama **C3 (Component)** detalha a organização interna do contêiner **`SIAES.API` (.NET 10 / ASP.NET Core)** para os módulos de **Clientes** e **Veículos**:
- **Apresentação (`SIAES.API`)**: Controladores REST que recebem as requisições HTTP, executam autenticação JWT e delegam a execução.
- **Camada Aplicação (`Application`)**: Casos de uso especializados que orquestram validações, consultas e comandos de escrita.
- **Camada Domínio (`Domain`)**: Entidades ricas (`Cliente`, `Veiculo`) e Value Objects imutáveis (`Documento`, `Placa`, `Endereco`) encapsulando invariantes de negócio.
- **Camada Infraestrutura (`Infrastructure`)**: Repositórios de persistência, mapeamentos ORM com o Entity Framework Core e conversão criptográfica determinística (AES-256) para conformidade com a LGPD.
- **Banco de Dados (`SIAES_DB`)**: Armazenamento relacional PostgreSQL 16 com integridade referencial estrita e índices únicos B-Tree.

---

## 2. Diagrama C3 — Componentes de Cliente e Veículo

```mermaid
flowchart TD
    %% Ator / Consumidor
    Usuario["Usuários da Oficina\n[Persons]\nAtendentes e Administradores"]:::person

    %% Fronteira do Sistema SIAES (API Backend)
    subgraph SIAESBoundary ["SIAES [API Backend — ASP.NET Core]"]
        
        %% Camada de Apresentação
        subgraph CamadaApresentacao ["Camada Apresentação"]
            ControllerVeic["Controller de Veículos\n[Veiculo]\nRecebe requisições HTTP e\nencaminha as operações."]:::component
            ControllerCli["Controller de Clientes\n[Cliente]\nRecebe requisições HTTP e\nencaminha as operações."]:::component
        end

        %% Camada Aplicação
        subgraph CamadaAplicacao ["Camada Aplicação [Container]"]
            CasosUsoVeic["Casos de Uso\n[Veiculo]\nOrquestra as operações de\ngestão de veículos."]:::component
            CasosUsoCli["Casos de Uso\n[Cliente]\nOrquestra as operações de\ngestão de clientes."]:::component
        end

        %% Camada Domínio
        subgraph CamadaDominio ["Camada Domínio [Container]"]
            DominioVeic["Domínio\n[Veículo]\nRepresenta as regras e os\nconceitos de negócio dos veículos."]:::component
            DominioCli["Domínio\n[Cliente]\nRepresenta as regras e os conceitos\nde negócio dos clientes."]:::component
        end

        %% Camada Infraestrutura
        subgraph CamadaInfra ["Camada Infraestrutura [Container]"]
            PersistenciaVeic["Persistência\n[Veiculos]\nImplementa o acesso e a\npersistência dos dados de veículos."]:::component
            PersistenciaCli["Persistência\n[Clientes]\nImplementa o acesso e a\npersistência dos dados de clientes."]:::component
            EFCore["Entity Framework Core\n[Entity Framework]\nMapeamento objeto-relacional (ORM)\ne conversões de criptografia LGPD"]:::component
        end

    end

    %% Banco de Dados Externo
    DB[("SIAES_DB\n[Container: PostgreSQL 16]\nBanco de dados relacional do sistema SIAES\nTabelas Clientes e Veiculos com índices únicos")]:::db

    %% Relações e Fluxo (exatamente conforme o modelo C3)
    Usuario -->|"HTTP / REST\n(JSON, Bearer Token)"| ControllerVeic
    Usuario -->|"HTTP / REST\n(JSON, Bearer Token)"| ControllerCli

    ControllerVeic -->|"Invoca operações"| CasosUsoVeic
    ControllerCli -->|"Invoca operações"| CasosUsoCli

    CasosUsoVeic -->|"Executa regras de negócio"| DominioVeic
    CasosUsoCli -->|"Executa regras de negócio"| DominioCli

    DominioVeic -->|"Persistência via Portas"| PersistenciaVeic
    DominioCli -->|"Persistência via Portas"| PersistenciaCli

    PersistenciaVeic -->|"Operações de dados"| EFCore
    PersistenciaCli -->|"Operações de dados"| EFCore

    EFCore -->|"SQL / TCP 5432"| DB

    %% Estilos visuais C4 (alinhados ao Draw.io)
    classDef person fill:#083F75,stroke:#06315C,color:#ffffff;
    classDef component fill:#1061B0,stroke:#0D5091,color:#ffffff;
    classDef db fill:#23A2D9,stroke:#0E7DAD,color:#ffffff;
```

---

## 3. Catálogo Detalhado de Componentes

### 3.1. Camada de Apresentação (`src/SIAES.API`)

| Componente | Tipo / Arquivo | Responsabilidade |
| :--- | :--- | :--- |
| **`Controller de Veículos`** | [`VeiculosController.cs`](file:///home/marcoswferreira/repos/tech-challenge/src/SIAES.API/Controllers/v1/VeiculosController.cs) | Endpoints REST `/api/v1/Veiculos` para cadastro, consulta por ID, listagem paginada, atualização cadastral e remoção lógica (soft delete). |
| **`Controller de Clientes`** | [`ClientesController.cs`](file:///home/marcoswferreira/repos/tech-challenge/src/SIAES.API/Controllers/v1/ClientesController.cs) | Endpoints REST `/api/v1/Clientes` para registro de clientes pessoa física/jurídica, consultas, atualização e exclusão lógica. |

### 3.2. Camada de Aplicação (`src/Application`)

| Componente | Casos de Uso Implementados | Responsabilidade |
| :--- | :--- | :--- |
| **`Casos de Uso [Veiculo]`** | • `CreateVeiculoUseCase`<br>• `GetVeiculoByIdUseCase`<br>• `GetVeiculosPagedUseCase`<br>• `UpdateVeiculoUseCase`<br>• `DeleteVeiculoUseCase` | Validação de existência do cliente proprietário (`ClienteId`), verificação de unicidade da placa no banco, coordenação de transações e transformação de entidades em DTOs mascarados. |
| **`Casos de Uso [Cliente]`** | • `CreateClienteUseCase`<br>• `GetClienteByIdUseCase`<br>• `GetClientesPagedUseCase`<br>• `UpdateClienteUseCase`<br>• `DeleteClienteUseCase` | Validação estrutural de entrada (`ClienteValidator`), verificação de documento duplicado (CPF/CNPJ) e montagem de saídas DTO mascaradas em conformidade com a LGPD. |

### 3.3. Camada de Domínio (`src/Domain`)

| Componente | Elementos e Value Objects | Responsabilidade |
| :--- | :--- | :--- |
| **`Domínio [Veículo]`** | Entidade [`Veiculo.cs`](file:///home/marcoswferreira/repos/tech-challenge/src/Domain/Entities/Veiculo.cs)<br>Value Object [`Placa.cs`](file:///home/marcoswferreira/repos/tech-challenge/src/Domain/ValueObjects/Placa.cs)<br>Porta [`IVeiculoRepository.cs`](file:///home/marcoswferreira/repos/tech-challenge/src/Domain/Repositories/IVeiculoRepository.cs) | Modela o veículo atendido na oficina. O Value Object `Placa` valida os formatos brasileiro tradicional (`AAA-9999`) e Mercosul (`AAA9A99`), além de expor as variantes formatada e mascarada (`ABC***3`). |
| **`Domínio [Cliente]`** | Raiz de Agregado [`Cliente.cs`](file:///home/marcoswferreira/repos/tech-challenge/src/Domain/Entities/Cliente.cs)<br>Value Object [`Documento.cs`](file:///home/marcoswferreira/repos/tech-challenge/src/Domain/ValueObjects/Documento.cs)<br>Value Object [`Endereco.cs`](file:///home/marcoswferreira/repos/tech-challenge/src/Domain/ValueObjects/Endereco.cs)<br>Porta [`IClienteRepository.cs`](file:///home/marcoswferreira/repos/tech-challenge/src/Domain/Repositories/IClienteRepository.cs) | Raiz de agregado que encapsula a coleção de veículos de um proprietário. O Value Object `Documento` calcula os dígitos verificadores para CPF (11 dígitos) e CNPJ (14 dígitos), rejeitando documentos inválidos com `DomainException`. |

### 3.4. Camada de Infraestrutura (`src/Infrastructure`)

| Componente | Tipo / Arquivo | Responsabilidade |
| :--- | :--- | :--- |
| **`Persistência [Veiculos]`** | [`VeiculoRepository.cs`](file:///home/marcoswferreira/repos/tech-challenge/src/Infrastructure/Repositories/VeiculoRepository.cs) | Implementação concreta de `IVeiculoRepository` com consultas otimizadas, filtros de exclusão lógica (`IsActive`) e busca por placa criptografada. |
| **`Persistência [Clientes]`** | [`ClienteRepository.cs`](file:///home/marcoswferreira/repos/tech-challenge/src/Infrastructure/Repositories/ClienteRepository.cs) | Implementação concreta de `IClienteRepository` com suporte a paginação `PagedResult<Cliente>`, busca por documento criptografado e carregamento de veículos vinculados. |
| **`Entity Framework Core`** | [`ApplicationDbContext.cs`](file:///home/marcoswferreira/repos/tech-challenge/src/Infrastructure/Database/ApplicationDbContext.cs)<br>[`DeterministicEncryption.cs`](file:///home/marcoswferreira/repos/tech-challenge/src/Infrastructure/Security/DeterministicEncryption.cs) | ORM configurado com migrações e `ValueConverter` que aplica criptografia determinística AES-256 no armazenamento de `Documento` e `Placa`, viabilizando índices únicos e buscas diretas no PostgreSQL sem expor texto plano no disco. |

---

## 4. Diagramas de Sequência

### 4.1. Cadastro de Veículo com Validação de Integridade e Placa

```mermaid
sequenceDiagram
    autonumber
    actor Atendente as Atendente da Oficina
    participant Controller as Controller de Veículos
    participant UseCase as CreateVeiculoUseCase
    participant RepoCliente as IClienteRepository
    participant RepoVeic as IVeiculoRepository
    participant PlacaVO as Placa (Value Object)
    participant VeicEntity as Veiculo (Entidade)
    participant EF as Entity Framework Core
    participant DB as SIAES_DB (PostgreSQL)

    Atendente->>Controller: POST /api/v1/Veiculos { clienteId, placa, marca, modelo, ano }
    Controller->>UseCase: ExecuteAsync(input, cancellationToken)
    
    %% Validação do Proprietário
    UseCase->>RepoCliente: GetByIdAsync(input.ClienteId)
    RepoCliente-->>UseCase: cliente (ou null se não encontrado)
    alt Cliente não encontrado
        UseCase-->>Controller: DomainException("Cliente não encontrado.")
        Controller-->>Atendente: 422 Unprocessable Entity
    end

    %% Validação da Placa
    UseCase->>PlacaVO: new Placa(input.Placa)
    alt Formato de Placa inválido
        PlacaVO-->>UseCase: DomainException("Placa inválida.")
        UseCase-->>Controller: 422 Unprocessable Entity
    end

    %% Unicidade da Placa
    UseCase->>RepoVeic: ExistsByPlacaAsync(placaVO.Valor)
    RepoVeic-->>UseCase: false (não duplicada)

    %% Instanciação e Persistência
    UseCase->>VeicEntity: new Veiculo(clienteId, placaVO, marca, modelo, ano)
    UseCase->>RepoVeic: AddAsync(veiculo)
    RepoVeic->>EF: DbContext.Veiculos.Add(veiculo)
    EF->>DB: INSERT INTO Veiculos (Criptografa Placa via AES-256)
    DB-->>EF: 201 Created (Id gerado)
    
    UseCase-->>Controller: VeiculoOutput (Placa mascarada: "ABC***3")
    Controller-->>Atendente: 201 Created (Location: /api/v1/Veiculos/{id})
```

---

## 5. Arquivo Draw.io Nativo

O diagrama original editável está disponível em:
- 📁 [`docs/architecture/drawio/C4_ClientesVeiculos_Component_C3.drawio`](drawio/C4_ClientesVeiculos_Component_C3.drawio)
