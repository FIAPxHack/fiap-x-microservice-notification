using Microsoft.Extensions.Logging;
using NotificationService.Domain.Interfaces.Services;

namespace NotificationService.Infrastructure.Email;

/// <summary>
/// Implementação do serviço de envio de email via SMTP
/// Atualmente em modo de simulação. Para produção, implementar integração SMTP real.
/// </summary>
public class SmtpEmailService : IEmailService
{
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(ILogger<SmtpEmailService> logger)
    {
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        // Simular delay de rede
        await Task.Delay(100);

        _logger.LogInformation(
            "📧 [EMAIL SIMULADO] Para: {To} | Assunto: {Subject} | Corpo: {Body}",
            to, subject, body.Length > 50 ? body.Substring(0, 50) + "..." : body);

        _logger.LogInformation(
            "SIMULAÇÃO: Email enviado com sucesso para {To}",
            to);
    }
}
