# Bem-vindo à Documentação do SIAES (18SOAT Tech Challenge)

Nesta documentação você encontrará toda a arquitetura técnica, modelagem de domínio com Domain-Driven Design (DDD), diagramas C4 e decisões arquiteturais (ADRs) adotadas no projeto **SIAES**.

---

## 🏛️ Arquitetura e Diagramas C4

| Documento | Nível / Foco | Descrição |
| :--- | :--- | :--- |
| **[Arquitetura e Estrutura da Solução](ddd/architecture.md)** | Visão Geral | Padrão Clean Architecture, divisão em camadas, regras de dependência, catálogo de casos de uso e estratégia de testes |
| **[C3 — Ordem de Serviço](architecture/c3-ordem-servico.md)** | Nível C3 (Componentes) | Diagrama C3 detalhando os componentes do contêiner API que sustentam o agregado Ordem de Serviço, catálogo de componentes e fluxos de orquestração |
| **[C3 — Gerenciamento de Estado](architecture/c3-gerenciamento-estado.md)** | Nível C3 (Componentes) | Diagrama C3 da Máquina de Estados da OS, regras de transição, invariantes, auditoria em banco relacional e tratamento de exceções de domínio |

---

## 📐 Domain-Driven Design (DDD)

| Documento | Descrição |
| :--- | :--- |
| **[Linguagem Ubíqua](ddd/linguagem-ubiqua.md)** | Glossário formal compartilhado entre especialistas de negócio e desenvolvedores |
| **[Domain Storytelling](ddd/domain-storytelling.md)** | Narrativas e diagramas dos fluxos operacionais da oficina (*Cenários 1, 2 e 3*) |
| **[Event Storming](ddd/event-storming.md)** | Mapeamento de eventos de domínio, comandos, agregados e reações do sistema |

---

## 📋 Architectural Decision Records (ADRs)

| ADR | Título | Status |
| :--- | :--- | :--- |
| **[ADR 001](adrs/adr-001-escolha-do-banco-de-dados.md)** | Seleção do Banco de Dados Relacional (PostgreSQL 16 com EF Core 10) | Aprovado |
