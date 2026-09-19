# Linguagem Ubíqua

A **Linguagem Ubíqua** é o vocabulário compartilhado entre os especialistas do negócio (equipe da oficina) e a equipe de desenvolvimento. Utilizaremos os termos abaixo para garantir que todas as comunicações, documentações e o próprio código do sistema usem a mesma terminologia, evitando ambiguidades.

## Atores
*   **Cliente**: Proprietário do veículo que necessita de manutenção e contrata o serviço da oficina.
*   **Atendente**: Funcionário responsável pelo primeiro contato com o cliente, criação da Ordem de Serviço, formalização de orçamentos e entrega do veículo.
*   **Mecânico**: Especialista técnico responsável por avaliar o veículo, realizar diagnósticos, requisitar peças e executar a manutenção.

## Entidades e Conceitos Principais
*   **Veículo**: O automóvel que receberá a manutenção. É identificado no sistema através de placa, marca, modelo e ano.
*   **Ordem de Serviço (OS)**: O coração do sistema. É o registro que acompanha todo o ciclo de vida de um atendimento. Contém os dados do cliente, veículo, serviços, peças, orçamento e o seu estado (Status) atual.
*   **Diagnóstico**: Processo no qual o mecânico avalia o veículo para identificar as causas de um problema e determinar quais serviços e peças serão necessários para o reparo.
*   **Serviço**: Ação específica de manutenção ou mão de obra que será executada no veículo (ex: troca de óleo, alinhamento, balanceamento).
*   **Peça / Insumo**: Componente físico (ex: filtro de ar) ou material de consumo (ex: óleo do motor) necessário para a execução de um serviço.
*   **Estoque**: Sistema de controle da quantidade física das peças e insumos disponíveis na oficina. Toda peça utilizada em uma OS deve dar baixa no estoque.
*   **Orçamento**: O cálculo do custo total previsto para uma OS, que soma os valores dos serviços e das peças/insumos. O orçamento precisa ser aprovado pelo cliente antes da execução do trabalho.

## Status da Ordem de Serviço (OS)
Os estados permitidos para uma Ordem de Serviço durante o seu ciclo de vida são restritos aos seguintes:

*   **Recebida**: Estado inicial logo após o cadastro da OS pelo atendente.
*   **Em diagnóstico**: O mecânico está ativamente avaliando o veículo para identificar os serviços e peças necessárias.
*   **Aguardando aprovação**: O diagnóstico foi concluído, o orçamento foi gerado e enviado ao cliente, e a oficina aguarda o "De Acordo".
*   **Em execução**: O cliente aprovou o orçamento e o mecânico está ativamente realizando o trabalho no veículo.
*   **Finalizada**: O mecânico concluiu todos os serviços listados na OS. O veículo está pronto para ser retirado.
*   **Entregue**: O cliente realizou o pagamento/retirada do veículo, e a OS foi formalmente encerrada pelo atendente.
