using System.Collections.Concurrent;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces.Repositories;

namespace NotificationService.Infrastructure.Persistence;

/// <summary>
/// Implementação em memória do repositório de notificações para desenvolvimento e testes.
/// </summary>
public class InMemoryNotificationRepository : INotificationRepository
{
    private readonly ConcurrentDictionary<string, NotificationHistory> _notifications = new();

    public Task<NotificationHistory> SaveAsync(NotificationHistory notification)
    {
        if (notification == null)
            throw new ArgumentNullException(nameof(notification));

        _notifications.TryAdd(notification.Id, notification);
        return Task.FromResult(notification);
    }

    public Task<NotificationHistory?> GetByIdAsync(string id)
    {
        _notifications.TryGetValue(id, out var notification);
        return Task.FromResult(notification);
    }

    public Task<IEnumerable<NotificationHistory>> GetByUserIdAsync(string userId)
    {
        var notifications = _notifications.Values
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .AsEnumerable();

        return Task.FromResult(notifications);
    }

    public Task UpdateAsync(NotificationHistory notification)
    {
        if (notification == null)
            throw new ArgumentNullException(nameof(notification));

        _notifications[notification.Id] = notification;
        return Task.CompletedTask;
    }
}
