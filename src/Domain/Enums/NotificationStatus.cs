namespace NotificationService.Domain.Enums;

/// <summary>
/// Status de uma notificação
/// </summary>
public enum NotificationStatus
{
    /// <summary>
    /// Notificação pendente de envio
    /// </summary>
    Pending,
    
    /// <summary>
    /// Notificação enviada com sucesso
    /// </summary>
    Sent,
    
    /// <summary>
    /// Falha no envio da notificação
    /// </summary>
    Failed
}
