namespace NotificationService.Domain.Enums;

/// <summary>
/// Tipos de notificação suportados pelo sistema
/// </summary>
public enum NotificationType
{
    /// <summary>
    /// Notificação de início de processamento de vídeo
    /// </summary>
    VideoProcessingStarted,
    
    /// <summary>
    /// Notificação de conclusão de processamento de vídeo
    /// </summary>
    VideoProcessingCompleted,
    
    /// <summary>
    /// Notificação de falha no processamento de vídeo
    /// </summary>
    VideoProcessingFailed,
    
    /// <summary>
    /// Notificação geral do sistema
    /// </summary>
    General
}
