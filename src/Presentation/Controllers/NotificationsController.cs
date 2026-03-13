using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;

namespace NotificationService.Presentation.Controllers;

/// <summary>
/// Controller para gerenciamento de notificações
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class NotificationsController : ControllerBase
{
    private readonly ISendNotificationUseCase _sendNotificationUseCase;
    private readonly IGetUserNotificationsUseCase _getUserNotificationsUseCase;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(
        ISendNotificationUseCase sendNotificationUseCase,
        IGetUserNotificationsUseCase getUserNotificationsUseCase,
        ILogger<NotificationsController> logger)
    {
        _sendNotificationUseCase = sendNotificationUseCase;
        _getUserNotificationsUseCase = getUserNotificationsUseCase;
        _logger = logger;
    }

    /// <summary>
    /// Envia uma notificação por e-mail
    /// </summary>
    /// <param name="request">Dados da notificação</param>
    /// <returns>Resultado do envio</returns>
    [HttpPost("send")]
    [ProducesResponseType(typeof(NotificationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendNotification([FromBody] NotificationRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Requisição inválida para envio de notificação");
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Recebida requisição de envio de notificação para {Email}", request.Email);

        var result = await _sendNotificationUseCase.ExecuteAsync(request);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Retorna o histórico de notificações de um usuário
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <returns>Lista de notificações do usuário</returns>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(IEnumerable<NotificationHistoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUserNotifications([FromRoute] string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest(new { message = "UserId é obrigatório" });
        }

        _logger.LogInformation("Buscando notificações para o usuário {UserId}", userId);

        var notifications = await _getUserNotificationsUseCase.ExecuteAsync(userId);
        return Ok(notifications);
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Health()
    {
        return Ok(new
        {
            status = "healthy",
            service = "notification-service",
            version = "2.0.0",
            architecture = "clean-architecture",
            timestamp = DateTime.UtcNow
        });
    }
}
