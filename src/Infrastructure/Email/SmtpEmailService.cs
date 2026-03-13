using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NotificationService.Domain.Interfaces.Services;

namespace NotificationService.Infrastructure.Email;

/// <summary>
/// Implementação do serviço de envio de email via SMTP
/// Atualmente em modo de simulação. Para produção, descomentar código SMTP real.
/// </summary>
public class SmtpEmailService : IEmailService
{
    private readonly ILogger<SmtpEmailService> _logger;
    private readonly IConfiguration _configuration;

    public SmtpEmailService(ILogger<SmtpEmailService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        // MODO SIMULAÇÃO (desenvolvimento)
        await SimulateSendEmailAsync(to, subject, body);

        // MODO PRODUÇÃO (descomentar quando configurar SMTP real)
        // await SendRealEmailAsync(to, subject, body);
    }

    private async Task SimulateSendEmailAsync(string to, string subject, string body)
    {
        // Simular delay de rede
        await Task.Delay(100);

        _logger.LogInformation(
            "📧 [EMAIL SIMULADO] Para: {To} | Assunto: {Subject} | Corpo: {Body}",
            to, subject, body.Length > 50 ? body.Substring(0, 50) + "..." : body);
    }

    // Implementação real para produção (descomentar quando necessário)
    /*
    private async Task SendRealEmailAsync(string to, string subject, string body)
    {
        try
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            var smtpUsername = _configuration["EmailSettings:Username"] ?? "";
            var smtpPassword = _configuration["EmailSettings:Password"] ?? "";
            var fromEmail = _configuration["EmailSettings:FromEmail"] ?? "noreply@fiapx.com";
            var fromName = _configuration["EmailSettings:FromName"] ?? "FIAP X";

            using var client = new SmtpClient(smtpServer, smtpPort);
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };
            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage);

            _logger.LogInformation("Email enviado com sucesso para {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar email para {To}", to);
            throw;
        }
    }
    */
}
