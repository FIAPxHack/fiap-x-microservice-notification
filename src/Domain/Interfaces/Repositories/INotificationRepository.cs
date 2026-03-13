using NotificationService.Domain.Entities;

namespace NotificationService.Domain.Interfaces.Repositories;

/// <summary>
/// Interface do repositório de notificações (contrato de domínio)
/// </summary>
public interface INotificationRepository
{
    /// <summary>
    /// Salva uma notificação no repositório
    /// </summary>
    Task<NotificationHistory> SaveAsync(NotificationHistory notification);
    
    /// <summary>
    /// Busca uma notificação por ID
    /// </summary>
    Task<NotificationHistory?> GetByIdAsync(string id);
    
    /// <summary>
    /// Busca todas as notificações de um usuário
    /// </summary>
    Task<IEnumerable<NotificationHistory>> GetByUserIdAsync(string userId);
    
    /// <summary>
    /// Atualiza uma notificação existente
    /// </summary>
    Task UpdateAsync(NotificationHistory notification);
}
