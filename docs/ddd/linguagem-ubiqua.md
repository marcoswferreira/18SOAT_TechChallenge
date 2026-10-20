# Linguagem Ubíqua

A **Linguagem Ubíqua** é o vocabulário compartilhado entre os especialistas do negócio (equipe da oficina) e a equipe de desenvolvimento. Os termos abaixo são usados de forma idêntica na conversa com o negócio, na documentação ([`README.md`](../../README.md) e [`architecture.md`](architecture.md)) e no código (nomes de entidades, métodos, enums e endpoints), evitando ambiguidades.

Na coluna **No código**, o nome entre crases é o identificador real usado na solução.

---

## Atores e Perfis de Acesso

Os atores abaixo são papéis de negócio. No sistema, o acesso é controlado por **perfis** (`Roles`).

| Ator (negócio) | Descrição | Perfil no sistema |
| :--- | :--- | :--- |
| **Cliente** | Proprietário do veículo que necessita de manutenção e contrata o serviço da oficina. Não possui login: acompanha a OS pela consulta pública e por e-mail. | — |
| **Atendente** | Funcionário responsável pelo primeiro contato com o cliente, abertura da Ordem de Serviço, envio do orçamento e entrega do veículo. | `Operacional` |
| **Mecânico** | Especialista técnico que avalia o veículo, faz o diagnóstico, informa peças e serviços e executa a manutenção. | `Operacional` |
| **Gerente** | Responsável pela gestão operacional e do estoque. | `Manager` |
| **Administrador** | Gestão completa do sistema, dos usuários e dos cadastros. | `Admin` |
| **Usuário** | Perfil básico, criado por padrão em novos cadastros. | `User` |

> `Atendente` e `Mecânico` são papéis de negócio distintos que compartilham o mesmo perfil de acesso `Operacional`.

---

## Entidades e Conceitos Principais

| Termo | Definição | No código |
| :--- | :--- | :--- |
| **Cliente** | Pessoa física ou jurídica dona de um ou mais veículos. É identificado por **Documento** e pode ter e-mail e telefone para contato. | `Cliente` |
| **Documento** | CPF ou CNPJ do cliente, validado pelos dígitos verificadores. Dado sensível (LGPD): armazenado criptografado e exibido mascarado (ex.: `529.***.***-25`). | `Documento`, `TipoDocumento` (`CPF`, `CNPJ`) |
| **Endereço** | Rua, número, complemento, bairro, cidade, estado e CEP do cliente. | `Endereco` |
| **Veículo** | Automóvel que receberá a manutenção, vinculado a um cliente. É identificado por **Placa**, marca, modelo e ano. | `Veiculo` |
| **Placa** | Identificação do veículo, aceita nos padrões antigo (`ABC-1234`) e Mercosul (`ABC1D23`). Dado sensível: armazenada criptografada e exibida mascarada. | `Placa` |
| **Ordem de Serviço (OS)** | Coração do sistema. Registro que acompanha todo o ciclo de vida de um atendimento: cliente, veículo, serviços, peças, valor do orçamento e **Status** atual. | `OrdemServico` |
| **Diagnóstico** | Avaliação do veículo pelo mecânico para identificar o problema e definir quais serviços e peças serão necessários. | `IniciarDiagnostico()` |
| **Serviço** | Tipo de serviço do catálogo da oficina (ex.: troca de óleo e filtros, alinhamento e balanceamento). | `Servico` |
| **Item de Serviço** | Serviço efetivamente incluído em uma OS, com descrição, quantidade e valor unitário. | `ItemServico` |
| **Peça / Insumo** | Componente (ex.: filtro de ar) ou material de consumo (ex.: óleo do motor) necessário para executar um serviço. Cadastrado como **Item de Estoque**. | `ItemEstoque` |
| **Item de Peça** | Peça incluída em uma OS, vinculada a um Item de Estoque, com quantidade e valor unitário. | `ItemPeca` |
| **Unidade de Medida** | Forma de controlar a quantidade de um item: `Unidade` (somente inteiros), `Litro` ou `Quilo` (aceitam frações). | `UnidadeMedida` |
| **Estoque** | Controle das quantidades de peças e insumos da oficina. Toda peça usada em uma OS reduz o estoque ao final do serviço. | `ItemEstoque.QuantidadeEmEstoque` |
| **Reserva** | Quantidade de uma peça comprometida com uma OS, ainda não consumida. Impede que duas OS usem o mesmo saldo. | `ItemEstoque.QuantidadeReservada`, `Reservar()` |
| **Saldo Disponível** | O que pode ser reservado: estoque físico menos o que já está reservado. | `ItemEstoque.QuantidadeDisponivel` |
| **Liberar Reserva** | Devolver ao saldo disponível a quantidade reservada, quando a OS é cancelada. | `LiberarReserva()` |
| **Baixa** | Consumo definitivo da peça ao finalizar o serviço: reduz a reserva e o estoque físico. | `BaixarReserva()` |
| **Entrada de Estoque** | Registro de reposição que aumenta a quantidade em estoque de um item. | `RegistrarEntradaEstoqueUseCase` |
| **Orçamento** | Valor total previsto da OS: soma dos itens de serviço e dos itens de peça. Só pode ser gerado se o valor for maior que zero e precisa ser aprovado pelo cliente antes da execução. | `OrdemServico.ValorTotal`, `GerarEEnviarOrcamento()` |
| **Histórico de Status** | Registro de cada mudança de status da OS (status anterior, novo status e data/hora). | `RegistroStatus` |
| **Notificação** | E-mail enviado ao cliente em momentos-chave da OS (ver tabela abaixo). Se o cliente não tem e-mail ou o envio falha, a operação de negócio não é afetada. | `IOrdemServicoNotificationService` |
| **Consulta Pública de Status** | Acompanhamento da OS pelo cliente, sem login, informando o ID da OS ou o CPF/CNPJ. | `GetStatusOrdemServicoUseCase` |

---

## Status da Ordem de Serviço (OS)

Os estados permitidos durante o ciclo de vida da OS são exatamente os abaixo. Qualquer outra transição é recusada.

| Status | No código | Significado |
| :--- | :--- | :--- |
| **Recebida** | `Recebida` | Estado inicial, logo após o atendente cadastrar a OS. |
| **Em diagnóstico** | `EmDiagnostico` | O mecânico está avaliando o veículo para identificar os serviços e peças necessários. |
| **Aguardando aprovação** | `AguardandoAprovacao` | O diagnóstico foi concluído, o orçamento foi gerado e enviado ao cliente, e a oficina aguarda o "de acordo". |
| **Em execução** | `EmExecucao` | O cliente aprovou o orçamento e o mecânico está realizando o trabalho no veículo. |
| **Finalizada** | `Finalizada` | O mecânico concluiu todos os serviços e as peças tiveram baixa no estoque. O veículo está pronto para retirada. |
| **Entregue** | `Entregue` | O veículo foi entregue ao cliente e a OS foi encerrada pelo atendente. |
| **Cancelada** | `Cancelada` | O cliente rejeitou o orçamento. A OS é encerrada e as reservas de peças são liberadas. |

### Transições permitidas

| De | Para | Ação de negócio |
| :--- | :--- | :--- |
| `Recebida` | `EmDiagnostico` | Iniciar diagnóstico |
| `EmDiagnostico` | `AguardandoAprovacao` | Gerar orçamento |
| `AguardandoAprovacao` | `EmExecucao` | Cliente aprova o orçamento |
| `AguardandoAprovacao` | `Cancelada` | Cliente rejeita o orçamento |
| `EmExecucao` | `Finalizada` | Finalizar serviço |
| `Finalizada` | `Entregue` | Registrar entrega |
| `EmExecucao` | `AguardandoAprovacao` | Solicitar aprovação adicional (`POST /api/v1/ordens-servico/{id}/solicitar-aprovacao-adicional`) |

---

## Notificações ao Cliente

| Momento | Assunto do e-mail |
| :--- | :--- |
| OS criada (`Recebida`) | Veículo Recebido |
| Orçamento gerado (`AguardandoAprovacao`) | Orçamento Disponível para Aprovação (com tabela de serviços, peças e valor total) |
| Orçamento aprovado (`EmExecucao`) | Orçamento Aprovado |
| Orçamento rejeitado (`Cancelada`) | Orçamento Rejeitado |
| Serviço finalizado (`Finalizada`) | Veículo Pronto para Retirada |
| Veículo entregue (`Entregue`) | Veículo Entregue |
| Serviço finalizado (`Finalizada`) | Veículo Pronto para Retirada |
| Veículo entregue (`Entregue`) | Veículo Entregue |

