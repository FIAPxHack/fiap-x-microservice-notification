using Microsoft.Extensions.Logging;
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Exceptions;
using NotificationService.Domain.Interfaces.Services;

namespace NotificationService.Application.UseCases;

/// <summary>
/// Caso de uso: Enviar notificação por email
/// </summary>
public class SendNotificationUseCase : ISendNotificationUseCase
{
    private readonly IEmailService _emailService;
    private readonly ILogger<SendNotificationUseCase> _logger;

    public SendNotificationUseCase(
        IEmailService emailService,
        ILogger<SendNotificationUseCase> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<NotificationResponseDto> ExecuteAsync(NotificationRequestDto request)
    {
        try
        {
            // 1. Gerar subject se não fornecido
            var subject = string.IsNullOrWhiteSpace(request.Subject)
                ? GetDefaultSubject(request.Type)
                : request.Subject;

            // 2. Criar entidade de domínio
            var notification = new NotificationHistory(
                request.UserId,
                request.Email,
                subject,
                request.Message,
                request.Type
            );

            // 3. Validar email
            if (!notification.IsValidEmail())
            {
                throw new InvalidEmailException(request.Email);
            }

            // 4. Enviar email
            _logger.LogInformation(
                "Enviando notificação {NotificationId} para {Email}",
                notification.Id, notification.Email);

            try
            {
                await _emailService.SendEmailAsync(
                    notification.Email,
                    notification.Subject,
                    notification.Message
                );

                _logger.LogInformation(
                    "Notificação {NotificationId} enviada com sucesso para {Email}",
                    notification.Id, notification.Email);

                return new NotificationResponseDto
                {
                    Success = true,
                    Message = "Notificação enviada com sucesso",
                    NotificationId = notification.Id
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao enviar email para {Email}", notification.Email);

                throw new NotificationException(
                    $"Falha ao enviar notificação para {notification.Email}", ex);
            }
        }
        catch (InvalidEmailException ex)
        {
            _logger.LogWarning(ex, "Email inválido fornecido: {Email}", request.Email);
            return new NotificationResponseDto
            {
                Success = false,
                Message = ex.Message
            };
        }
        catch (NotificationException)
        {
            // Re-lançar exceções de notificação para que os testes possam capturar
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao processar notificação");
            return new NotificationResponseDto
            {
                Success = false,
                Message = "Erro interno ao processar notificação"
            };
        }
    }

    private static string GetDefaultSubject(NotificationType type)
    {
        return type switch
        {
            NotificationType.VideoProcessingStarted => "🎬 Processamento de vídeo iniciado - FIAP X",
            NotificationType.VideoProcessingCompleted => "✅ Processamento de vídeo concluído - FIAP X",
            NotificationType.VideoProcessingFailed => "❌ Falha no processamento de vídeo - FIAP X",
            NotificationType.General => "📧 Notificação FIAP X",
            _ => "FIAP X - Notificação"
        };
    }
}
