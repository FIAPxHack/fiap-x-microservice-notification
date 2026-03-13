namespace NotificationService.Models;

public class NotificationRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
}

public enum NotificationType
{
    VideoProcessingStarted,
    VideoProcessingCompleted,
    VideoProcessingFailed,
    General
}
