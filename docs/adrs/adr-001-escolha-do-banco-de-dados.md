# ADR 001: Seleção do Banco de Dados Relacional (PostgreSQL)

* **Status**: Aceito
* **Data**: 2026-19-07
* **Autores**: Grupo X
* **Contexto Técnico**: .NET 10, Monólito em Camadas (DDD), Docker / Docker Compose

---

## 1. Contexto e Problema

O projeto **SIAES** (*Sistema Integrado de Atendimento e Execução de Serviços*) é a primeira versão (MVP) do back-end para gestão de uma oficina mecânica de médio porte. O sistema gerencia o ciclo de vida completo de Ordens de Serviço (OS), cadastro de clientes e veículos, precificação automatizada de orçamentos e controle estrito de estoque de peças e insumos.

Os requisitos técnicos do desafio exigem um **back-end monolítico** estruturado sob os princípios de **Domain-Driven Design (DDD)**. Embora a escolha da tecnologia de persistência seja livre, a arquitetura exige uma justificativa técnica formal para a escolha do banco de dados.

Os principais desafios de persistência do domínio incluem:

1. **Consistência estrita em operações de estoque e orçamento**: Operações de aprovação de orçamento exigem a reserva imediata e a baixa transacional de peças e insumos para evitar vendas duplicadas ou inconsistência de saldo.
2. **Modelo relacional fortemente conectado**: O domínio possui relacionamentos complexos e rígidos (Cliente 1:N Veículos, Cliente 1:N OS, OS 1:N Peças/Serviços).
3. **Execução local e conteinerização**: O ambiente precisa ser orquestrado via `docker-compose.yml` para facilidade de execução local pelos avaliadores.

---

## 2. Decisão

Decidimos adotar o **PostgreSQL (versão 16-alpine)** como o Sistema Gerenciador de Banco de Dados (SGBD) relacional principal do **SIAES**, integrado ao **Entity Framework Core (EF Core 10)** na camada de Infraestrutura.

---

## 3. Justificativa Técnica

### 3.1. Garantias ACID e Integridade Transacional

O fluxo de aprovação de orçamentos e execução de serviços necessita de suporte nativo a transações **ACID** (*Atomacidade, Consistência, Isolamento e Durabilidade*). O PostgreSQL garante que ações compostas (ex.: alterar o status da OS para "Em Execução" + debitar itens do Estoque) ocorram de forma atômica, revertendo a operação (*rollback*) em caso de falha técnica ou falta de peças.

### 3.2. Mapeamento Natural do Modelo Tático do DDD

Os conceitos de **Entidades**, **Objetos de Valor** e **Agregados** do DDD alinham-se perfeitamente com a estrutura relacional do PostgreSQL:

* A entidade **`OrdemDeServico`** atua como raiz do agregado, mapeando suas tabelas filhas (`ServicosPrestados`, `PecasUtilizadas`) com chaves estrangeiras e integridade referencial.
* Chaves primárias UUID/Guid garantem a identidade imutável das entidades antes mesmo da persistência.

### 3.3. Recursos Híbridos (Suporte a JSONB)

Apesar de ser um banco relacional robusto, o PostgreSQL possui suporte nativo ao tipo **`JSONB`**. Isso possibilita armazenar *snapshots* temporários de diagnósticos técnicos não estruturados ou logs de alteração de status sem a necessidade de introduzir um banco NoSQL secundário nesta fase do MVP, reduzindo a complexidade operacional do monólito.

### 3.4. Facilidade de Conteinerização e Baixo Consumo de Recursos

A utilização da imagem oficial `postgres:16-alpine` no **`docker-compose.yml`** garante um container leve (< 100 MB), inicialização rápida e suporte a *healthchecks* nativos (`pg_isready`), atendendo diretamente ao requisito de execução local simples exigido no Tech Challenge.

### 3.5. Compatibilidade e Ecossistema .NET

A biblioteca `Npgsql.EntityFrameworkCore.PostgreSQL` oferece integração madura e de altíssima performance com o .NET 10, permitindo aplicar migrações automatizadas (*EF Core Migrations*), consultas LINQ otimizadas e Inversão de Dependência na camada de Infraestrutura.

---

## 4. Alternativas Consideradas e Motivos do Descarte

| Alternativa | Motivo do Descarte |
| :--- | :--- |
| **MongoDB (NoSQL Orientado a Documentos)** | Descartado porque a inconsistência eventual e a falta de integridade referencial nativa dificultariam o controle rígido de transações financeiras e baixa de estoque do MVP. |
| **MySQL / MariaDB** | Descartado por possuir menor flexibilidade na manipulação de dados semi-estruturados (`JSONB`) e menores recursos avançados de índices concorrentes em comparação ao PostgreSQL. |
| **Microsoft SQL Server** | Descartado pelo maior consumo de memória RAM e CPU no ambiente Docker local durante os testes avaliativos, além de restrições de licença em ambientes de produção. |

---

## 5. Consequências

### Positivas

* ✅ **Consistência de dados assegurada**: Elimina o risco de inconsistências no estoque ou orçamentos duplicados.
* ✅ **Alinhamento com o DDD**: Facilita a implementação do padrão *Repository* na camada de Infraestrutura sem vazar detalhes técnicos para o Domínio.
* ✅ **Pronto para conteinerização**: Subida automatizada do banco e criação de tabelas via Docker Compose.

### Riscos e Mitigações

* ⚠️ **Risco**: Necessidade de gerenciar migrações de esquema do banco de dados durante a evolução do sistema.
  * **Mitigação**: Uso de *EF Core Migrations* executadas automaticamente na inicialização da aplicação em ambiente de desenvolvimento.
* ⚠️ **Risco**: Gargalo de I/O em consultas complexas no histórico de OSs.
  * **Mitigação**: Criação de índices específicos nas colunas de busca frequente (`ClienteId`, `StatusOS`, `PlacaVeiculo`).
