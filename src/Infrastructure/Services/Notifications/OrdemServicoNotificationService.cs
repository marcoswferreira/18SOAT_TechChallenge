using System.Globalization;
using System.Net;
using System.Text;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Notifications;

public class OrdemServicoNotificationService(
    IResendEmailService emailService,
    IClienteRepository clienteRepository,
    IVeiculoRepository veiculoRepository,
    ILogger<OrdemServicoNotificationService> logger) : IOrdemServicoNotificationService
{
    private static readonly CultureInfo PtBrCulture = new("pt-BR");
    private readonly IResendEmailService _emailService = emailService;
    private readonly IClienteRepository _clienteRepository = clienteRepository;
    private readonly IVeiculoRepository _veiculoRepository = veiculoRepository;
    private readonly ILogger<OrdemServicoNotificationService> _logger = logger;

    private readonly record struct NotificationContext(
        string Assunto,
        string Titulo,
        string StatusBadge,
        string BadgeCor,
        string MensagemPrincipal,
        string ConteudoAdicional = ""
    );

    public Task NotificarOrdemServicoCriadaAsync(
        OrdemServico os,
        Cliente? cliente = null,
        Veiculo? veiculo = null,
        CancellationToken cancellationToken = default)
        => NotificarAsync(os, cliente, veiculo, new NotificationContext(
            $"[SIAES] Ordem de Serviço #{FormatarId(os.Id)} - Veículo Recebido",
            "Veículo Recebido na Oficina", "Recebida", "#2563eb",
            "Seu veículo deu entrada com sucesso em nossa oficina mecânica. Uma nova Ordem de Serviço foi gerada e a equipe técnica dará início ao diagnóstico.",
            CriarCardAlerta("#2563eb", "Próximo passo:", "Assim que nossos mecânicos finalizarem a inspeção do veículo, você receberá um e-mail com o orçamento detalhado (serviços e peças) para sua aprovação.")
        ), cancellationToken);

    public Task NotificarOrcamentoParaAprovacaoAsync(
        OrdemServico os,
        Cliente? cliente = null,
        Veiculo? veiculo = null,
        CancellationToken cancellationToken = default)
    {
        var sbItens = new StringBuilder();

        AdicionarTabelaItens(sbItens, "Serviços e Mão de Obra", "Descrição",
            os.Servicos.Select(s => (s.Descricao, s.Quantidade.ToString(), s.ValorUnitario, s.ValorTotal)));

        AdicionarTabelaItens(sbItens, "Peças e Componentes", "Peça",
            os.Pecas.Select(p => (p.NomePeca, p.Quantidade.ToString("0.###", PtBrCulture), p.ValorUnitario, p.ValorTotal)));

        sbItens.Append($"""
            <div style="background-color: #f0fdf4; border: 1px solid #bbf7d0; border-radius: 6px; padding: 14px 18px; margin: 20px 0; text-align: right;">
                <span style="font-size: 14px; color: #166534; margin-right: 12px;">Valor Total do Orçamento:</span>
                <span style="font-size: 20px; font-weight: 700; color: #15803d;">{os.ValorTotal.ToString("C", PtBrCulture)}</span>
            </div>
            """);

        sbItens.Append(CriarCardAlerta("#f59e0b", "Ação necessária:", "O serviço aguarda sua aprovação para ser iniciado. Favor entrar em contato com a oficina para autorizar a execução ou esclarecer eventuais dúvidas.", "#fffbeb", "#92400e"));

        return NotificarAsync(os, cliente, veiculo, new NotificationContext(
            $"[SIAES] Orçamento Disponível para Aprovação - OS #{FormatarId(os.Id)}",
            "Orçamento Pronto para Aprovação", "Aguardando Aprovação", "#d97706",
            "O diagnóstico do seu veículo foi concluído e os itens necessários foram orçados pela equipe técnica. Confira os detalhes abaixo:",
            sbItens.ToString()
        ), cancellationToken);
    }

    public Task NotificarOrcamentoAprovadoAsync(
        OrdemServico os,
        Cliente? cliente = null,
        Veiculo? veiculo = null,
        CancellationToken cancellationToken = default)
        => NotificarAsync(os, cliente, veiculo, new NotificationContext(
            $"[SIAES] Orçamento Aprovado - OS #{FormatarId(os.Id)}",
            "Orçamento Aprovado com Sucesso", "Em Execução", "#7c3aed",
            "Confirmamos a aprovação do orçamento. Nossa equipe de mecânicos já iniciou a execução dos serviços contratados.",
            CriarCardAlerta("#7c3aed", "Acompanhamento:", "Você será notificado por este mesmo canal assim que o veículo passar por todas as etapas de revisão e testes de qualidade.")
        ), cancellationToken);

    public Task NotificarOrcamentoRejeitadoAsync(
        OrdemServico os,
        Cliente? cliente = null,
        Veiculo? veiculo = null,
        CancellationToken cancellationToken = default)
        => NotificarAsync(os, cliente, veiculo, new NotificationContext(
            $"[SIAES] Orçamento Rejeitado - OS #{FormatarId(os.Id)}",
            "Orçamento Rejeitado / OS Cancelada", "Cancelada", "#dc2626",
            "Registramos a rejeição do orçamento da Ordem de Serviço. A OS foi cancelada e as reservas de peças foram liberadas no estoque.",
            CriarCardAlerta("#dc2626", "", "O veículo encontra-se disponível na oficina para retirada. Caso queira conversar sobre ajustes no orçamento, nossa equipe está à disposição.", "#fef2f2", "#991b1b")
        ), cancellationToken);

    public Task NotificarServicoFinalizadoAsync(
        OrdemServico os,
        Cliente? cliente = null,
        Veiculo? veiculo = null,
        CancellationToken cancellationToken = default)
        => NotificarAsync(os, cliente, veiculo, new NotificationContext(
            $"[SIAES] Veículo Pronto para Retirada - OS #{FormatarId(os.Id)}",
            "Serviço Concluído - Veículo Pronto!", "Pronto para Retirada", "#16a34a",
            "Temos uma excelente notícia! Todos os serviços da Ordem de Serviço foram concluídos com sucesso e o veículo foi aprovado em todos os testes de qualidade.",
            $"""
            <div style="background-color: #f0fdf4; border: 1px solid #bbf7d0; border-radius: 6px; padding: 16px; margin: 16px 0;">
                <p style="margin: 0 0 8px 0; font-size: 14px; color: #166534;">
                    <strong>Valor Final dos Serviços:</strong> {os.ValorTotal.ToString("C", PtBrCulture)}
                </p>
                <p style="margin: 0; font-size: 14px; color: #166534;">
                    <strong>Local de Retirada:</strong> Oficina SIAES - Atendimento de Segunda a Sexta, das 08h às 18h.
                </p>
            </div>
            """
        ), cancellationToken);

    public Task NotificarVeiculoEntregueAsync(
        OrdemServico os,
        Cliente? cliente = null,
        Veiculo? veiculo = null,
        CancellationToken cancellationToken = default)
        => NotificarAsync(os, cliente, veiculo, new NotificationContext(
            $"[SIAES] Veículo Entregue - OS #{FormatarId(os.Id)}",
            "Veículo Entregue com Sucesso", "Entregue", "#059669",
            "Confirmamos a entrega do seu veículo. Agradecemos pela confiança em nossos serviços!",
            CriarCardAlerta("#059669", "", "A equipe SIAES permanece sempre à sua disposição para futuras manutenções preventivas e revisões do seu veículo.")
        ), cancellationToken);

    private async Task NotificarAsync(
        OrdemServico os,
        Cliente? cliente,
        Veiculo? veiculo,
        NotificationContext context,
        CancellationToken cancellationToken)
    {
        var (c, v) = await ObterClienteEVeiculoAsync(os, cliente, veiculo, cancellationToken);
        if (c == null || string.IsNullOrWhiteSpace(c.Email))
        {
            _logger.LogInformation("Cliente da OS {OrdemServicoId} não possui e-mail cadastrado ou não foi localizado. Notificação ignorada.", os.Id);
            return;
        }

        var body = GerarTemplateBase(os, c, v, context);

        try
        {
            await _emailService.SendAsync(c.Email, context.Assunto, body, cancellationToken);
            _logger.LogInformation("E-mail de notificação '{Assunto}' enviado com sucesso para {Destinatario} (OS: {Id})", context.Assunto, c.Email, os.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao enviar e-mail de notificação '{Assunto}' para {Destinatario} da OS {Id}. O fluxo principal não foi interrompido.", context.Assunto, c.Email, os.Id);
        }
    }

    private async Task<(Cliente? cliente, Veiculo? veiculo)> ObterClienteEVeiculoAsync(
        OrdemServico os,
        Cliente? clienteParam,
        Veiculo? veiculoParam,
        CancellationToken cancellationToken)
    {
        var cliente = clienteParam ?? os.Cliente ?? await _clienteRepository.GetByIdAsync(os.ClienteId, cancellationToken);
        var veiculo = veiculoParam ?? os.Veiculo ?? await _veiculoRepository.GetByIdAsync(os.VeiculoId, cancellationToken);
        return (cliente, veiculo);
    }

    private static string FormatarId(Guid id) => id.ToString()[..8].ToUpperInvariant();

    private static string CriarCardAlerta(string corBorda, string destaque, string texto, string corFundo = "#f8fafc", string corTexto = "#334155")
        => $"""
            <div style="background-color: {corFundo}; border-left: 4px solid {corBorda}; padding: 12px 16px; margin: 16px 0; border-radius: 4px;">
                <p style="margin: 0; font-size: 14px; color: {corTexto}; line-height: 1.5;">
                    {(string.IsNullOrEmpty(destaque) ? "" : $"<strong>{destaque}</strong> ")}{texto}
                </p>
            </div>
            """;

    private static void AdicionarTabelaItens(
        StringBuilder sb,
        string tituloSecao,
        string tituloItem,
        IEnumerable<(string Nome, string Quantidade, decimal ValorUnitario, decimal ValorTotal)> itens)
    {
        var lista = itens.ToList();
        if (lista.Count == 0) return;

        sb.Append($"""
            <h4 style="color: #1e293b; margin: 20px 0 8px 0; font-size: 15px; border-bottom: 2px solid #e2e8f0; padding-bottom: 6px;">{tituloSecao}</h4>
            <table style="width: 100%; border-collapse: collapse; font-size: 13px; margin-bottom: 16px;">
                <thead>
                    <tr style="background-color: #f1f5f9; text-align: left; color: #475569;">
                        <th style="padding: 8px 12px; border: 1px solid #e2e8f0;">{tituloItem}</th>
                        <th style="padding: 8px 12px; border: 1px solid #e2e8f0; text-align: center;">Qtd</th>
                        <th style="padding: 8px 12px; border: 1px solid #e2e8f0; text-align: right;">Unitário</th>
                        <th style="padding: 8px 12px; border: 1px solid #e2e8f0; text-align: right;">Total</th>
                    </tr>
                </thead>
                <tbody>
            """);

        foreach (var item in lista)
        {
            sb.Append($"""
                <tr>
                    <td style="padding: 8px 12px; border: 1px solid #e2e8f0;">{WebUtility.HtmlEncode(item.Nome)}</td>
                    <td style="padding: 8px 12px; border: 1px solid #e2e8f0; text-align: center;">{item.Quantidade}</td>
                    <td style="padding: 8px 12px; border: 1px solid #e2e8f0; text-align: right;">{item.ValorUnitario.ToString("C", PtBrCulture)}</td>
                    <td style="padding: 8px 12px; border: 1px solid #e2e8f0; text-align: right; font-weight: bold;">{item.ValorTotal.ToString("C", PtBrCulture)}</td>
                </tr>
                """);
        }

        sb.Append("</tbody></table>");
    }

    private static string GerarTemplateBase(
        OrdemServico os,
        Cliente cliente,
        Veiculo? veiculo,
        NotificationContext context)
    {
        var nomeCliente = WebUtility.HtmlEncode(cliente.Nome);
        var veiculoInfo = veiculo != null
            ? $"{WebUtility.HtmlEncode(veiculo.Marca)} {WebUtility.HtmlEncode(veiculo.Modelo)} (Placa: {WebUtility.HtmlEncode(veiculo.Placa.Valor)})"
            : "Veículo cadastrado na OS";

        return $$"""
            <!DOCTYPE html>
            <html lang="pt-BR">
            <head>
                <meta charset="utf-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>{{context.Titulo}}</title>
            </head>
            <body style="margin: 0; padding: 24px 10px; background-color: #f1f5f9; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; color: #1e293b;">
                <div style="max-width: 620px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -2px rgba(0, 0, 0, 0.1);">
                    <!-- Header -->
                    <div style="background-color: #0f172a; padding: 24px; text-align: center;">
                        <h1 style="margin: 0; color: #ffffff; font-size: 20px; font-weight: 700; letter-spacing: 0.5px;">SIAES</h1>
                        <p style="margin: 4px 0 0 0; color: #94a3b8; font-size: 13px;">Sistema Inteligente de Automecânica e Estoque</p>
                    </div>

                    <!-- Conteúdo Principal -->
                    <div style="padding: 28px 24px;">
                        <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px;">
                            <span style="font-size: 13px; font-weight: 600; color: #64748b;">OS #{{os.Id.ToString()[..8].ToUpperInvariant()}}</span>
                            <span style="background-color: {{context.BadgeCor}}; color: #ffffff; padding: 4px 10px; border-radius: 9999px; font-size: 12px; font-weight: 600;">
                                {{context.StatusBadge}}
                            </span>
                        </div>

                        <h2 style="margin: 0 0 16px 0; color: #0f172a; font-size: 18px; font-weight: 700;">{{context.Titulo}}</h2>

                        <p style="margin: 0 0 14px 0; font-size: 15px; color: #334155; line-height: 1.5;">
                            Olá, <strong>{{nomeCliente}}</strong>,
                        </p>

                        <p style="margin: 0 0 20px 0; font-size: 14px; color: #475569; line-height: 1.5;">
                            {{context.MensagemPrincipal}}
                        </p>

                        <!-- Box do Veículo -->
                        <div style="background-color: #f8fafc; border: 1px solid #e2e8f0; border-radius: 6px; padding: 12px 16px; margin-bottom: 20px;">
                            <span style="font-size: 12px; color: #64748b; font-weight: 600; text-transform: uppercase;">Veículo Associado</span>
                            <div style="font-size: 14px; font-weight: 600; color: #1e293b; margin-top: 4px;">{{veiculoInfo}}</div>
                        </div>

                        {{context.ConteudoAdicional}}
                    </div>

                    <!-- Rodapé -->
                    <div style="background-color: #f8fafc; border-top: 1px solid #e2e8f0; padding: 18px 24px; text-align: center;">
                        <p style="margin: 0; font-size: 12px; color: #64748b;">
                            Este é um e-mail automático gerado pelo SIAES. Por favor, não responda diretamente a este e-mail.
                        </p>
                    </div>
                </div>
            </body>
            </html>
            """;
    }
}
