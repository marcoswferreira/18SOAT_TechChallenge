using Domain.Interfaces.Services;
using Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Resend;

namespace Infrastructure.Services;

public class ResendEmailService(IResend resend, IOptions<ResendSettings> settings) : IResendEmailService
{
    private readonly ResendSettings _settings = settings.Value;

    public async Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        EmailMessage message = new()
        {
            From = _settings.FromEmail,
            To = to,
            Subject = subject,
            HtmlBody = body
        };

        ResendResponse<Guid> response = await resend.EmailSendAsync(message, cancellationToken);

        if (!response.Success)
        {
            throw new InvalidOperationException($"Erro ao enviar e-mail: {response.Exception?.Message}");
        }
    }
}
