# Bem-vindo à Documentação do SIAES (18SOAT Tech Challenge)

> 🎓 **Documento Oficial de Entrega (Fase 1)**: Para visualização ou exportação para PDF da entrega da Fase 1, acesse [`docs/ENTREGA_FASE1.md`](ENTREGA_FASE1.md).

Nesta documentação você encontrará toda a arquitetura técnica, modelagem de domínio com Domain-Driven Design (DDD), diagramas C4 e decisões arquiteturais (ADRs) adotadas no projeto **SIAES**.

---

## 🏛️ Arquitetura e Diagramas C4

A arquitetura do SIAES segue a metodologia **C4 Model** (Contexto, Contêineres e Componentes):

| Documento | Nível C4 | Descrição |
| :--- | :--- | :--- |
| **[C1 — Contexto do Sistema](architecture/c1-contexto.md)** | **Nível 1 (Context)** | Diagrama de Contexto: escopo do sistema SIAES, personas da oficina (Atendente, Mecânico, Administrador) e serviço externo de e-mails |
| **[C2 — Contêineres do Sistema](architecture/c2-container.md)** | **Nível 2 (Container)** | Diagrama de Contêineres: `SIAES.API` (.NET 10), `SIAES_DB` (PostgreSQL 16) e integrações via HTTP/REST, SQL e SMTP/API |
| **[C3 — Cliente e Veículo](architecture/c3-cliente-veiculo.md)** | **Nível 3 (Component)** | Diagrama de Componentes C3 alinhado ao Clean Architecture: Controllers, Casos de Uso, Domínio, Persistência e conformidade LGPD |
| **[C3 — Ordem de Serviço](architecture/c3-ordem-servico.md)** | **Nível 3 (Component)** | Diagrama C3 detalhando os componentes do contêiner API que sustentam o agregado Ordem de Serviço, estoque e mensageria |
| **[C3 — Gerenciamento de Estado](architecture/c3-gerenciamento-estado.md)** | **Nível 3 (Component)** | Diagrama C3 da Máquina de Estados da OS, regras de transição, invariantes, auditoria em banco relacional e exceções |
| **[C3 — Estoque](architecture/c3-estoque.md)** | **Nível 3 (Component)** | Diagrama C3 do Estoque: cadastro de itens (peças/insumos), entrada de saldo, reserva/liberação/baixa pela OS, concorrência otimista e carga inicial |
| **[Arquitetura e Estrutura da Solução](ddd/architecture.md)** | Visão Geral | Padrão Clean Architecture, divisão em camadas, regras de dependência, catálogo de casos de uso e testes |

> 📁 **Arquivos nativos editáveis do Draw.io** disponíveis na pasta [`docs/architecture/drawio/`](architecture/drawio/):
> - [`C4_ClientesVeiculos_Contexto_C1.drawio`](architecture/drawio/C4_ClientesVeiculos_Contexto_C1.drawio) — Contexto do Sistema (C1)
> - [`C4_ClientesVeiculos_Container_C2.drawio`](architecture/drawio/C4_ClientesVeiculos_Container_C2.drawio) — Contêineres do Sistema (C2)
> - [`C4_ClientesVeiculos_Component_C3.drawio`](architecture/drawio/C4_ClientesVeiculos_Component_C3.drawio) — Componentes de Cliente e Veículo (C3)
> - [`C4_OrdemServico_Component_C3.drawio`](architecture/drawio/C4_OrdemServico_Component_C3.drawio) — Componentes de Ordem de Serviço (C3)
> - [`C4_GerenciamentoEstado_Component_C3.drawio`](architecture/drawio/C4_GerenciamentoEstado_Component_C3.drawio) — Componentes do Gerenciamento de Estado / FSM (C3)
> - [`C4_Estoque_Component_C3.drawio`](architecture/drawio/C4_Estoque_Component_C3.drawio) — Componentes de Estoque (C3)

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

---

## ⚡ API, Swagger e Testes End-to-End (Insomnia & Thunder Client)

| Recurso | Localização | Descrição |
| :--- | :--- | :--- |
| **Collection Insomnia (Nativa)** | [`docs/insomnia/Insomnia_SIAES_Collection.json`](insomnia/Insomnia_SIAES_Collection.json) | Suíte E2E nativa do Insomnia com 51 requisições encadeadas e variáveis dinâmicas |
| **Guia Insomnia** | [`docs/insomnia/README.md`](insomnia/README.md) | Instruções para importar e rodar a suíte no Insomnia |
| **Collection Postman / Insomnia** | [`docs/insomnia/Postman_SIAES_Collection.json`](insomnia/Postman_SIAES_Collection.json) | Formato Postman v2.1 importável no Insomnia com scripts de teste |
| **Collection Thunder Client** | [`docs/thunder-client/thunder-collection_SIAES.json`](thunder-client/thunder-collection_SIAES.json) | Suíte completa para Thunder Client (VS Code) |
| **Guia Thunder Client** | [`docs/thunder-client/README.md`](thunder-client/README.md) | Passo a passo para importar e rodar no Thunder Client |


