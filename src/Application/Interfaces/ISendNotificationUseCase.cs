using NotificationService.Application.DTOs;

namespace NotificationService.Application.Interfaces;

/// <summary>
/// Interface para o caso de uso de envio de notificação
/// </summary>
public interface ISendNotificationUseCase
{
    Task<NotificationResponseDto> ExecuteAsync(NotificationRequestDto request);
}
