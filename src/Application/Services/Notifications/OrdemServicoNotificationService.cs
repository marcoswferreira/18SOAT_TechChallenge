using System.Globalization;
using System.Net;
using System.Text;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Application.Services.Notifications;

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

    public async Task NotificarOrdemServicoCriadaAsync(
        OrdemServico os,
        Cliente? cliente = null,
        Veiculo? veiculo = null,
        CancellationToken cancellationToken = default)
    {
        var (c, v) = await ObterClienteEVeiculoAsync(os, cliente, veiculo, cancellationToken);
        if (!ValidarEmailCliente(c, os.Id)) return;

        var subject = $"[SIAES] Ordem de Serviço #{FormatarId(os.Id)} - Veículo Recebido";
        var body = GerarTemplateBase(
            titulo: "Veículo Recebido na Oficina",
            badgeStatus: "Recebida",
            badgeCor: "#2563eb",
            cliente: c!,
            veiculo: v,
            os: os,
            mensagemPrincipal: "Seu veículo deu entrada com sucesso em nossa oficina mecânica. Uma nova Ordem de Serviço foi gerada e a equipe técnica dará início ao diagnóstico.",
            conteudoAdicional: """
                <div style="background-color: #f8fafc; border-left: 4px solid #2563eb; padding: 12px 16px; margin: 16px 0; border-radius: 4px;">
                    <p style="margin: 0; font-size: 14px; color: #334155;">
                        <strong>Próximo passo:</strong> Assim que nossos mecânicos finalizarem a inspeção do veículo, você receberá um e-mail com o orçamento detalhado (serviços e peças) para sua aprovação.
                    </p>
                </div>
                """
        );

        await EnviarEmailComTratamentoErroAsync(c!.Email!, subject, body, os.Id, cancellationToken);
    }

    public async Task NotificarOrcamentoParaAprovacaoAsync(
        OrdemServico os,
        Cliente? cliente = null,
        Veiculo? veiculo = null,
        CancellationToken cancellationToken = default)
    {
        var (c, v) = await ObterClienteEVeiculoAsync(os, cliente, veiculo, cancellationToken);
        if (!ValidarEmailCliente(c, os.Id)) return;

        var subject = $"[SIAES] Orçamento Disponível para Aprovação - OS #{FormatarId(os.Id)}";

        var sbItens = new StringBuilder();

        // Tabela de Serviços
        if (os.Servicos.Any())
        {
            sbItens.Append("""
                <h4 style="color: #1e293b; margin: 20px 0 8px 0; font-size: 15px; border-bottom: 2px solid #e2e8f0; padding-bottom: 6px;">Serviços e Mão de Obra</h4>
                <table style="width: 100%; border-collapse: collapse; font-size: 13px; margin-bottom: 16px;">
                    <thead>
                        <tr style="background-color: #f1f5f9; text-align: left; color: #475569;">
                            <th style="padding: 8px 12px; border: 1px solid #e2e8f0;">Descrição</th>
                            <th style="padding: 8px 12px; border: 1px solid #e2e8f0; text-align: center;">Qtd</th>
                            <th style="padding: 8px 12px; border: 1px solid #e2e8f0; text-align: right;">Unitário</th>
                            <th style="padding: 8px 12px; border: 1px solid #e2e8f0; text-align: right;">Total</th>
                        </tr>
                    </thead>
                    <tbody>
                """);

            foreach (var servico in os.Servicos)
            {
                sbItens.Append($"""
                    <tr>
                        <td style="padding: 8px 12px; border: 1px solid #e2e8f0;">{WebUtility.HtmlEncode(servico.Descricao)}</td>
                        <td style="padding: 8px 12px; border: 1px solid #e2e8f0; text-align: center;">{servico.Quantidade}</td>
                        <td style="padding: 8px 12px; border: 1px solid #e2e8f0; text-align: right;">{servico.ValorUnitario.ToString("C", PtBrCulture)}</td>
                        <td style="padding: 8px 12px; border: 1px solid #e2e8f0; text-align: right; font-weight: bold;">{servico.ValorTotal.ToString("C", PtBrCulture)}</td>
                    </tr>
                    """);
            }

            sbItens.Append("</tbody></table>");
        }

        // Tabela de Peças
        if (os.Pecas.Any())
        {
            sbItens.Append("""
                <h4 style="color: #1e293b; margin: 20px 0 8px 0; font-size: 15px; border-bottom: 2px solid #e2e8f0; padding-bottom: 6px;">Peças e Componentes</h4>
                <table style="width: 100%; border-collapse: collapse; font-size: 13px; margin-bottom: 16px;">
                    <thead>
                        <tr style="background-color: #f1f5f9; text-align: left; color: #475569;">
                            <th style="padding: 8px 12px; border: 1px solid #e2e8f0;">Peça</th>
                            <th style="padding: 8px 12px; border: 1px solid #e2e8f0; text-align: center;">Qtd</th>
                            <th style="padding: 8px 12px; border: 1px solid #e2e8f0; text-align: right;">Unitário</th>
                            <th style="padding: 8px 12px; border: 1px solid #e2e8f0; text-align: right;">Total</th>
                        </tr>
                    </thead>
                    <tbody>
                """);

            foreach (var peca in os.Pecas)
            {
                sbItens.Append($"""
                    <tr>
                        <td style="padding: 8px 12px; border: 1px solid #e2e8f0;">{WebUtility.HtmlEncode(peca.NomePeca)}</td>
                        <td style="padding: 8px 12px; border: 1px solid #e2e8f0; text-align: center;">{peca.Quantidade.ToString("0.###", PtBrCulture)}</td>
                        <td style="padding: 8px 12px; border: 1px solid #e2e8f0; text-align: right;">{peca.ValorUnitario.ToString("C", PtBrCulture)}</td>
                        <td style="padding: 8px 12px; border: 1px solid #e2e8f0; text-align: right; font-weight: bold;">{peca.ValorTotal.ToString("C", PtBrCulture)}</td>
                    </tr>
                    """);
            }

            sbItens.Append("</tbody></table>");
        }

        // Bloco Totalizador e Instruções
        sbItens.Append($"""
            <div style="background-color: #f0fdf4; border: 1px solid #bbf7d0; border-radius: 6px; padding: 14px 18px; margin: 20px 0; text-align: right;">
                <span style="font-size: 14px; color: #166534; margin-right: 12px;">Valor Total do Orçamento:</span>
                <span style="font-size: 20px; font-weight: 700; color: #15803d;">{os.ValorTotal.ToString("C", PtBrCulture)}</span>
            </div>

            <div style="background-color: #fffbeb; border-left: 4px solid #f59e0b; padding: 12px 16px; margin: 16px 0; border-radius: 4px;">
                <p style="margin: 0; font-size: 14px; color: #92400e; line-height: 1.5;">
                    <strong>Ação necessária:</strong> O serviço aguarda sua aprovação para ser iniciado. Favor entrar em contato com a oficina para autorizar a execução ou esclarecer eventuais dúvidas.
                </p>
            </div>
            """);

        var body = GerarTemplateBase(
            titulo: "Orçamento Pronto para Aprovação",
            badgeStatus: "Aguardando Aprovação",
            badgeCor: "#d97706",
            cliente: c!,
            veiculo: v,
            os: os,
            mensagemPrincipal: "O diagnóstico do seu veículo foi concluído e os itens necessários foram orçados pela equipe técnica. Confira os detalhes abaixo:",
            conteudoAdicional: sbItens.ToString()
        );

        await EnviarEmailComTratamentoErroAsync(c!.Email!, subject, body, os.Id, cancellationToken);
    }

    public async Task NotificarOrcamentoAprovadoAsync(
        OrdemServico os,
        Cliente? cliente = null,
        Veiculo? veiculo = null,
        CancellationToken cancellationToken = default)
    {
        var (c, v) = await ObterClienteEVeiculoAsync(os, cliente, veiculo, cancellationToken);
        if (!ValidarEmailCliente(c, os.Id)) return;

        var subject = $"[SIAES] Orçamento Aprovado - OS #{FormatarId(os.Id)}";
        var body = GerarTemplateBase(
            titulo: "Orçamento Aprovado com Sucesso",
            badgeStatus: "Em Execução",
            badgeCor: "#7c3aed",
            cliente: c!,
            veiculo: v,
            os: os,
            mensagemPrincipal: "Confirmamos a aprovação do orçamento. Nossa equipe de mecânicos já iniciou a execução dos serviços contratados.",
            conteudoAdicional: """
                <div style="background-color: #f8fafc; border-left: 4px solid #7c3aed; padding: 12px 16px; margin: 16px 0; border-radius: 4px;">
                    <p style="margin: 0; font-size: 14px; color: #334155;">
                        <strong>Acompanhamento:</strong> Você será notificado por este mesmo canal assim que o veículo passar por todas as etapas de revisão e testes de qualidade.
                    </p>
                </div>
                """
        );

        await EnviarEmailComTratamentoErroAsync(c!.Email!, subject, body, os.Id, cancellationToken);
    }

    public async Task NotificarOrcamentoRejeitadoAsync(
        OrdemServico os,
        Cliente? cliente = null,
        Veiculo? veiculo = null,
        CancellationToken cancellationToken = default)
    {
        var (c, v) = await ObterClienteEVeiculoAsync(os, cliente, veiculo, cancellationToken);
        if (!ValidarEmailCliente(c, os.Id)) return;

        var subject = $"[SIAES] Orçamento Rejeitado - OS #{FormatarId(os.Id)}";
        var body = GerarTemplateBase(
            titulo: "Orçamento Rejeitado / OS Cancelada",
            badgeStatus: "Cancelada",
            badgeCor: "#dc2626",
            cliente: c!,
            veiculo: v,
            os: os,
            mensagemPrincipal: "Registramos a rejeição do orçamento da Ordem de Serviço.",
            conteudoAdicional: """
                <div style="background-color: #fef2f2; border-left: 4px solid #dc2626; padding: 12px 16px; margin: 16px 0; border-radius: 4px;">
                    <p style="margin: 0; font-size: 14px; color: #991b1b;">
                        O veículo encontra-se disponível na oficina para retirada. Caso queira conversar sobre ajustes no orçamento, nossa equipe está à disposição.
                    </p>
                </div>
                """
        );

        await EnviarEmailComTratamentoErroAsync(c!.Email!, subject, body, os.Id, cancellationToken);
    }

    public async Task NotificarServicoFinalizadoAsync(
        OrdemServico os,
        Cliente? cliente = null,
        Veiculo? veiculo = null,
        CancellationToken cancellationToken = default)
    {
        var (c, v) = await ObterClienteEVeiculoAsync(os, cliente, veiculo, cancellationToken);
        if (!ValidarEmailCliente(c, os.Id)) return;

        var subject = $"[SIAES] Veículo Pronto para Retirada - OS #{FormatarId(os.Id)}";
        var body = GerarTemplateBase(
            titulo: "Serviço Concluído - Veículo Pronto!",
            badgeStatus: "Pronto para Retirada",
            badgeCor: "#16a34a",
            cliente: c!,
            veiculo: v,
            os: os,
            mensagemPrincipal: "Temos uma excelente notícia! Todos os serviços da Ordem de Serviço foram concluídos com sucesso e o veículo foi aprovado em todos os testes de qualidade.",
            conteudoAdicional: $"""
                <div style="background-color: #f0fdf4; border: 1px solid #bbf7d0; border-radius: 6px; padding: 16px; margin: 16px 0;">
                    <p style="margin: 0 0 8px 0; font-size: 14px; color: #166534;">
                        <strong>Valor Final dos Serviços:</strong> {os.ValorTotal.ToString("C", PtBrCulture)}
                    </p>
                    <p style="margin: 0; font-size: 14px; color: #166534;">
                        <strong>Local de Retirada:</strong> Oficina SIAES - Atendimento de Segunda a Sexta, das 08h às 18h.
                    </p>
                </div>
                """
        );

        await EnviarEmailComTratamentoErroAsync(c!.Email!, subject, body, os.Id, cancellationToken);
    }

    public async Task NotificarVeiculoEntregueAsync(
        OrdemServico os,
        Cliente? cliente = null,
        Veiculo? veiculo = null,
        CancellationToken cancellationToken = default)
    {
        var (c, v) = await ObterClienteEVeiculoAsync(os, cliente, veiculo, cancellationToken);
        if (!ValidarEmailCliente(c, os.Id)) return;

        var subject = $"[SIAES] Veículo Entregue - OS #{FormatarId(os.Id)}";
        var body = GerarTemplateBase(
            titulo: "Veículo Entregue com Sucesso",
            badgeStatus: "Entregue",
            badgeCor: "#059669",
            cliente: c!,
            veiculo: v,
            os: os,
            mensagemPrincipal: "Confirmamos a entrega do seu veículo. Agradecemos pela confiança em nossos serviços!",
            conteudoAdicional: """
                <div style="background-color: #f8fafc; border-left: 4px solid #059669; padding: 12px 16px; margin: 16px 0; border-radius: 4px;">
                    <p style="margin: 0; font-size: 14px; color: #334155;">
                        A equipe SIAES permanece sempre à sua disposição para futuras manutenções preventivas e revisões do seu veículo.
                    </p>
                </div>
                """
        );

        await EnviarEmailComTratamentoErroAsync(c!.Email!, subject, body, os.Id, cancellationToken);
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

    private bool ValidarEmailCliente(Cliente? cliente, Guid osId)
    {
        if (cliente == null || string.IsNullOrWhiteSpace(cliente.Email))
        {
            _logger.LogInformation("Cliente da OS {OrdemServicoId} não possui e-mail cadastrado ou não foi localizado. Notificação ignorada.", osId);
            return false;
        }

        return true;
    }

    private async Task EnviarEmailComTratamentoErroAsync(
        string destinatario,
        string assunto,
        string corpoHtml,
        Guid osId,
        CancellationToken cancellationToken)
    {
        try
        {
            await _emailService.SendAsync(destinatario, assunto, corpoHtml, cancellationToken);
            _logger.LogInformation("E-mail de notificação '{Assunto}' enviado com sucesso para {Destinatario} (OS: {Id})", assunto, destinatario, osId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao enviar e-mail de notificação '{Assunto}' para {Destinatario} da OS {Id}. O fluxo principal não foi interrompido.", assunto, destinatario, osId);
        }
    }

    private static string FormatarId(Guid id) => id.ToString()[..8].ToUpperInvariant();

    private static string GerarTemplateBase(
        string titulo,
        string badgeStatus,
        string badgeCor,
        Cliente cliente,
        Veiculo? veiculo,
        OrdemServico os,
        string mensagemPrincipal,
        string conteudoAdicional)
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
                <title>{{titulo}}</title>
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
                            <span style="background-color: {{badgeCor}}; color: #ffffff; padding: 4px 10px; border-radius: 9999px; font-size: 12px; font-weight: 600;">
                                {{badgeStatus}}
                            </span>
                        </div>

                        <h2 style="margin: 0 0 16px 0; color: #0f172a; font-size: 18px; font-weight: 700;">{{titulo}}</h2>

                        <p style="margin: 0 0 14px 0; font-size: 15px; color: #334155; line-height: 1.5;">
                            Olá, <strong>{{nomeCliente}}</strong>,
                        </p>

                        <p style="margin: 0 0 20px 0; font-size: 14px; color: #475569; line-height: 1.5;">
                            {{mensagemPrincipal}}
                        </p>

                        <!-- Box do Veículo -->
                        <div style="background-color: #f8fafc; border: 1px solid #e2e8f0; border-radius: 6px; padding: 12px 16px; margin-bottom: 20px;">
                            <span style="font-size: 12px; color: #64748b; font-weight: 600; text-transform: uppercase;">Veículo Associado</span>
                            <div style="font-size: 14px; font-weight: 600; color: #1e293b; margin-top: 4px;">{{veiculoInfo}}</div>
                        </div>

                        {{conteudoAdicional}}
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
