namespace NotificationService.Application.DTOs;

/// <summary>
/// DTO para resposta de envio de notificação
/// </summary>
public class NotificationResponseDto
{
    /// <summary>
    /// Indica se o envio foi bem-sucedido
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Mensagem descritiva do resultado
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// ID da notificação criada (se sucesso)
    /// </summary>
    public string? NotificationId { get; set; }
    
    /// <summary>
    /// Timestamp da resposta
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
