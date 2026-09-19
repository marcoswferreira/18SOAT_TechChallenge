# Arquitetura e Estrutura da Solução (SIAES)

Este documento descreve as decisões arquiteturais e a organização física e lógica da solução **SIAES**.

## 1. Padrão Arquitetural

A solução foi desenhada com base nos princípios da **Clean Architecture** (Arquitetura Limpa) e **Domain-Driven Design (DDD)**. O objetivo é manter o domínio isolado de frameworks, bancos de dados e interfaces externas, facilitando a manutenção e a testabilidade.

### Camadas

A separação de responsabilidades está refletida nos projetos da solução (dentro da pasta `src/`):

1. **Domain (`src/Domain`)**
   - **Responsabilidade**: Contém o coração da aplicação. Aqui ficam as Entidades, Value Objects, Interfaces de Repositórios e Regras de Negócio fundamentais.
   - **Dependências**: Não depende de nenhuma outra camada (Core).
   - **Status**: Em desenvolvimento. _[TODO: Documentar principais agregados e entidades do domínio]_

2. **Application (`src/Application`)**
   - **Responsabilidade**: Orquestra os fluxos de negócio. Contém os Casos de Uso (Use Cases), DTOs, interfaces de serviços externos e validadores de entrada.
   - **Dependências**: Depende do `Domain`.
   - **Status**: Em desenvolvimento. _[TODO: Listar os fluxos principais e commands/queries, se usar CQRS]_

3. **Infrastructure (`src/Infrastructure`)**
   - **Responsabilidade**: Implementa as interfaces definidas no Domain e Application. Responsável pela comunicação com o banco de dados (Entity Framework), chamadas HTTP externas, sistema de arquivos, filas, etc.
   - **Dependências**: Depende de `Domain` e `Application`.
   - **Status**: Em desenvolvimento. _[TODO: Documentar provedores externos e modelagem de dados]_

4. **API / Presentation (`src/SIAES.API`)**
   - **Responsabilidade**: Ponto de entrada da aplicação. Contém os Controllers (ou Minimal APIs), configurações de injeção de dependência (IoC), middlewares e Swagger.
   - **Dependências**: Depende de `Application` e `Infrastructure`.
   - **Status**: Em desenvolvimento. _[TODO: Documentar padrões de autenticação, endpoints principais e middlewares customizados]_

---

## 2. Tecnologias Utilizadas

- **Linguagem**: C# / .NET 10.0
- **Banco de Dados**: PostgreSQL (via Docker)
- **ORM**: Entity Framework Core
- **Testes**: xUnit + Coverlet
- **Análise de Qualidade**: SonarQube
- **Documentação da API**: Swagger / OpenAPI
- _[TODO: Cache (Redis), Autenticação (JWT/Identity), etc.]_

---

## 3. Estrutura de Testes

Os testes automatizados estão localizados na pasta `tests/` e seguem a pirâmide de testes:

- **UnitTests (`tests/UnitTests`)**: Testa o comportamento isolado das classes de `Domain` e `Application`, utilizando mocks para dependências externas.
- **IntegrationTests (`tests/IntegrationTests`)**: Testa a integração entre a `API`, o banco de dados e as respostas HTTP, validando o fluxo ponta-a-ponta (end-to-end local).
- _[TODO: Definir padrão de escrita de testes (ex: Given-When-Then, AAA), ferramentas de Mock (Moq/NSubstitute) e FluentAssertions]_

---

## 4. O que falta ser desenvolvido (Roadmap Técnico)

Esta seção lista os itens arquiteturais ou técnicos que ainda precisam ser definidos ou implementados:

- [ ] **Estratégia de Migrations**: Definir como as migrations do EF Core serão rodadas.
- [x] **Tratamento Global de Erros**: Implementar um `ExceptionMiddleware` padronizado para retornar erros consistentes (ex: RFC 7807 Problem Details).
- [ ] **Observabilidade e Logs**: Integrar ferramenta para tracing e logs centralizados.
- [ ] **Autenticação e Autorização**: Concluir a integração e configuração do JWT e controle de permissões por roles/policies.
- [x] **Pipelines de CI/CD**: Criar Github Actions para rodar os testes, a análise do SonarQube.
