using System.ComponentModel.DataAnnotations;
using NotificationService.Domain.Enums;

namespace NotificationService.Application.DTOs;

/// <summary>
/// DTO para requisição de envio de notificação
/// </summary>
public class NotificationRequestDto
{
    /// <summary>
    /// ID do usuário que receberá a notificação
    /// </summary>
    [Required(ErrorMessage = "UserId é obrigatório")]
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// Email do destinatário
    /// </summary>
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email em formato inválido")]
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Assunto da notificação (opcional - será gerado automaticamente se não fornecido)
    /// </summary>
    public string? Subject { get; set; }
    
    /// <summary>
    /// Mensagem da notificação
    /// </summary>
    [Required(ErrorMessage = "Mensagem é obrigatória")]
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// Tipo de notificação
    /// </summary>
    [Required]
    public required NotificationType Type { get; set; }
}
