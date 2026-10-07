namespace Domain.Interfaces.Services;

public interface IResendEmailService
{
    Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
}
