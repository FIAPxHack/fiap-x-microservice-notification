namespace NotificationService.Models;

public class NotificationResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? NotificationId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
