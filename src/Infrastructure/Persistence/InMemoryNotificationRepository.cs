using System.Collections.Concurrent;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces.Repositories;

namespace NotificationService.Infrastructure.Persistence;

/// <summary>
/// Implementação em memória do repositório de notificações para desenvolvimento e testes.
/// </summary>
public class InMemoryNotificationRepository : INotificationRepository
{
    private readonly ConcurrentBag<NotificationHistory> _notifications = new();

    public Task<NotificationHistory> SaveAsync(NotificationHistory notification)
    {
        ArgumentNullException.ThrowIfNull(notification);

        _notifications.Add(notification);
        return Task.FromResult(notification);
    }

    public Task<NotificationHistory?> GetByIdAsync(string id)
    {
        var notification = _notifications.FirstOrDefault(n => n.Id == id);
        return Task.FromResult(notification);
    }

    public Task<IEnumerable<NotificationHistory>> GetByUserIdAsync(string userId)
    {
        var notifications = _notifications
            .Where(n => n.UserId == userId)
            .OrderBy(n => n.CreatedAt)
            .AsEnumerable();

        return Task.FromResult(notifications);
    }

    public Task UpdateAsync(NotificationHistory notification)
    {
        ArgumentNullException.ThrowIfNull(notification);

        // Para atualizar em ConcurrentBag, precisamos encontrar e manter a referência
        // Como estamos trabalhando com referências, a atualização já foi feita no objeto
        return Task.CompletedTask;
    }
}
