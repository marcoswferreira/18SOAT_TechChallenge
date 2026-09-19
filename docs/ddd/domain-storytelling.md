# Domain Storytelling (As-Is)

Nesta página, documentamos o fluxo de negócio **atual (As-Is)** da oficina mecânica, antes da implementação do Sistema Integrado. Atualmente, o processo é feito de forma desorganizada, utilizando anotações manuais e planilhas.

## Atores
*   **Cliente**: Proprietário do veículo que necessita de manutenção.
*   **Atendente**: Responsável por receber o cliente e fazer as anotações iniciais.
*   **Mecânico**: Responsável pelo diagnóstico e execução dos serviços.

---

## Cenário 1: Chegada, Diagnóstico e Orçamento

![Fluxo Cenário 1: Chegada, Diagnóstico e Orçamento](./images/cenario1.png)

1. O **Cliente** chega à oficina e solicita um atendimento ao **Atendente**.
2. O **Atendente** anota os dados do **Cliente** e do veículo em uma *Ficha de Papel* ou *Planilha Genérica*. (Problema: Perda de histórico de clientes e veículos).
3. O **Atendente** repassa a *Ficha de Papel* para o **Mecânico** solicitando uma avaliação. (Problema: Erros na priorização dos atendimentos).
4. O **Mecânico** avalia o veículo, anota o diagnóstico e as peças necessárias na mesma *Ficha de Papel*.
5. O **Atendente** pega a *Ficha de Papel*, calcula os valores manualmente consultando outras planilhas ou catálogos e entra em contato com o **Cliente** (telefone/WhatsApp) para passar o orçamento e aguardar a autorização. (Problema: Ineficiência no fluxo de orçamentos e autorizações).

---

## Cenário 2: Execução do Serviço e Retirada de Peças

![Fluxo Cenário 2: Execução do Serviço e Retirada de Peças](./images/cenario2.png)

1. O **Cliente** autoriza o orçamento via telefone ou mensagem com o **Atendente**.
2. O **Atendente** avisa verbalmente ou devolve a *Ficha de Papel* ao **Mecânico** indicando que o serviço está aprovado.
3. O **Mecânico** vai até o estoque buscar as peças. Muitas vezes ele pega as peças sem dar baixa na *Planilha de Estoque*. (Problema: Falhas no controle de peças e insumos).
4. O **Mecânico** inicia a execução do serviço no veículo. O **Cliente**, caso ligue para saber do andamento, obriga o **Atendente** a ir fisicamente até a oficina perguntar ao **Mecânico** sobre o veículo. (Problema: Dificuldade em acompanhar o status dos serviços).

---

## Cenário 3: Entrega e Finalização

![Fluxo Cenário 3: Entrega e Finalização](./images/cenario3.png)

1. O **Mecânico** termina o serviço, guarda as ferramentas e avisa o **Atendente**.
2. O **Atendente** entra em contato com o **Cliente** avisando que o carro está pronto.
3. O **Cliente** chega na oficina, realiza o pagamento ao **Atendente**.
4. O **Atendente** arquiva a *Ficha de Papel* em uma gaveta ou pasta física, encerrando o atendimento.
