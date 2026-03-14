using NotificationService.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace NotificationService.Domain.Entities;

/// <summary>
/// Entidade de domínio que representa o histórico de uma notificação
/// </summary>
public class NotificationHistory
{
    public string Id { get; private set; }
    public string UserId { get; private set; }
    public string Email { get; private set; }
    public string Subject { get; private set; }
    public string Message { get; private set; }
    public NotificationType Type { get; private set; }
    public NotificationStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? SentAt { get; private set; }

    // Construtor privado para EF Core
    private NotificationHistory() 
    {
        Id = string.Empty;
        UserId = string.Empty;
        Email = string.Empty;
        Subject = string.Empty;
        Message = string.Empty;
    }

    // Construtor público para criação de novas notificações
    public NotificationHistory(
        string userId,
        string email,
        string subject,
        string message,
        NotificationType type)
    {
        Id = Guid.NewGuid().ToString();
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Subject = subject ?? throw new ArgumentNullException(nameof(subject));
        Message = message ?? string.Empty;
        Type = type;
        Status = NotificationStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marca a notificação como enviada com sucesso
    /// </summary>
    public void MarkAsSent()
    {
        Status = NotificationStatus.Sent;
        if (!SentAt.HasValue)
        {
            SentAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Marca a notificação como falha
    /// </summary>
    public void MarkAsFailed()
    {
        Status = NotificationStatus.Failed;
    }

    /// <summary>
    /// Valida se o email está em formato válido
    /// </summary>
    public bool IsValidEmail()
    {
        if (string.IsNullOrWhiteSpace(Email))
        {
            return false;
        }

        var parts = Email.Split('@');
        if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1]))
        {
            return false;
        }

        var domain = parts[1];
        if (domain.StartsWith('.') || domain.EndsWith('.') || !domain.Contains('.'))
        {
            return false;
        }

        return new EmailAddressAttribute().IsValid(Email);
    }
}
