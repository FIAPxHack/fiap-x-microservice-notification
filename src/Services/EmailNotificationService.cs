using NotificationService.Models;
using System.Collections.Concurrent;

namespace NotificationService.Services;

public class EmailNotificationService : INotificationService
{
    private readonly ILogger<EmailNotificationService> _logger;
    private readonly IConfiguration _configuration;
    
    private static readonly ConcurrentBag<NotificationHistory> _notificationHistory = new();

    public EmailNotificationService(ILogger<EmailNotificationService> _logger, IConfiguration configuration)
    {
        this._logger = _logger;
        _configuration = configuration;
    }

    public async Task<NotificationResponse> SendNotificationAsync(NotificationRequest request)
    {
        try
        {
            // Validações básicas
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return new NotificationResponse
                {
                    Success = false,
                    Message = "Email é obrigatório"
                };
            }

            if (string.IsNullOrWhiteSpace(request.Subject))
            {
                request.Subject = GetDefaultSubject(request.Type);
            }

            // Criar histórico
            var notification = new NotificationHistory
            {
                UserId = request.UserId,
                Email = request.Email,
                Subject = request.Subject,
                Type = request.Type,
                Status = NotificationStatus.Pending
            };

            // Simular envio de e-mail
            await SimulateSendEmailAsync(request);

            // Atualizar status
            notification.Status = NotificationStatus.Sent;
            notification.SentAt = DateTime.UtcNow;
            _notificationHistory.Add(notification);

            _logger.LogInformation(
                "Notificação enviada com sucesso. UserId: {UserId}, Email: {Email}, Type: {Type}",
                request.UserId, request.Email, request.Type);

            return new NotificationResponse
            {
                Success = true,
                Message = "Notificação enviada com sucesso",
                NotificationId = notification.Id
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar notificação para {Email}", request.Email);
            
            return new NotificationResponse
            {
                Success = false,
                Message = $"Erro ao enviar notificação: {ex.Message}"
            };
        }
    }

    public Task<IEnumerable<NotificationHistory>> GetUserNotificationsAsync(string userId)
    {
        var notifications = _notificationHistory
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .AsEnumerable();

        return Task.FromResult(notifications);
    }

    private async Task SimulateSendEmailAsync(NotificationRequest request)
    {
        await Task.Delay(100);

        _logger.LogInformation(
            "📧 [SIMULADO] E-mail enviado para {Email} - Assunto: {Subject}",
            request.Email, request.Subject);
    }

    private static string GetDefaultSubject(NotificationType type)
    {
        return type switch
        {
            NotificationType.VideoProcessingStarted => "Processamento de vídeo iniciado",
            NotificationType.VideoProcessingCompleted => "Processamento de vídeo concluído",
            NotificationType.VideoProcessingFailed => "Falha no processamento de vídeo",
            _ => "Notificação FIAP X"
        };
    }
}
