using NotificationService.Application.DTOs;

namespace NotificationService.Application.Interfaces;

/// <summary>
/// Interface para o caso de uso de busca de notificações de um usuário
/// </summary>
public interface IGetUserNotificationsUseCase
{
    Task<IEnumerable<NotificationHistoryDto>> ExecuteAsync(string userId);
}
