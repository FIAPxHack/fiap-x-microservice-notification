using Microsoft.Extensions.Logging;
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Interfaces.Repositories;

namespace NotificationService.Application.UseCases;

/// <summary>
/// Caso de uso: Buscar notificações de um usuário
/// </summary>
public class GetUserNotificationsUseCase : IGetUserNotificationsUseCase
{
    private readonly INotificationRepository _repository;
    private readonly ILogger<GetUserNotificationsUseCase> _logger;

    public GetUserNotificationsUseCase(
        INotificationRepository repository,
        ILogger<GetUserNotificationsUseCase> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<NotificationHistoryDto>> ExecuteAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            _logger.LogWarning("Tentativa de buscar notificações com userId vazio");
            return Enumerable.Empty<NotificationHistoryDto>();
        }

        _logger.LogInformation("Buscando notificações para o usuário {UserId}", userId);

        var notifications = await _repository.GetByUserIdAsync(userId);

        var dtos = notifications
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationHistoryDto
            {
                Id = n.Id,
                UserId = n.UserId,
                Email = n.Email,
                Subject = n.Subject,
                Message = n.Message,
                Type = n.Type,
                Status = n.Status,
                CreatedAt = n.CreatedAt,
                SentAt = n.SentAt
            });

        return dtos;
    }
}
