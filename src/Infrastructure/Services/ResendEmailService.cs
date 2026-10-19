using Domain.Interfaces.Services;
using Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Resend;

namespace Infrastructure.Services;

public class ResendEmailService(
    IResend resend,
    IOptions<ResendSettings> settings,
    ILogger<ResendEmailService> logger) : IResendEmailService
{
    private readonly ResendSettings _settings = settings.Value;
    private readonly ILogger<ResendEmailService> _logger = logger;

    public async Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        var fromEmail = string.IsNullOrWhiteSpace(_settings.FromEmail)
            ? "onboarding@resend.dev"
            : _settings.FromEmail;

        EmailMessage message = new()
        {
            From = fromEmail,
            To = to,
            Subject = subject,
            HtmlBody = body
        };

        try
        {
            ResendResponse<Guid> response = await resend.EmailSendAsync(message, cancellationToken);

            if (!response.Success)
            {
                _logger.LogWarning("Erro ao enviar e-mail via Resend para {To}: {Error}", to, response.Exception?.Message);
                throw new InvalidOperationException($"Erro ao enviar e-mail: {response.Exception?.Message}");
            }

            _logger.LogInformation("E-mail enviado com sucesso para {To} com o assunto '{Subject}'.", to, subject);
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            _logger.LogError(ex, "Exceção inesperada ao enviar e-mail para {To}", to);
            throw;
        }
    }
}
