namespace NotificationService.Domain.Interfaces.Services;

/// <summary>
/// Interface do serviço de envio de email (contrato de domínio)
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Envia um email
    /// </summary>
    /// <param name="to">Destinatário</param>
    /// <param name="subject">Assunto</param>
    /// <param name="body">Corpo do email</param>
    Task SendEmailAsync(string to, string subject, string body);
}
