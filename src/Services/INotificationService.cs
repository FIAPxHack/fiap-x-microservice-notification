using NotificationService.Models;

namespace NotificationService.Services;

public interface INotificationService
{
    Task<NotificationResponse> SendNotificationAsync(NotificationRequest request);
}
