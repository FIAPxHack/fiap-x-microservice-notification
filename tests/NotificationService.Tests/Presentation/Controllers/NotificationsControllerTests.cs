using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Enums;
using NotificationService.Presentation.Controllers;
using Xunit;

namespace NotificationService.Tests.Presentation.Controllers;

/// <summary>
/// Testes unitários para o NotificationsController
/// </summary>
public class NotificationsControllerTests
{
    private readonly Mock<ISendNotificationUseCase> _mockSendUseCase;
    private readonly Mock<IGetUserNotificationsUseCase> _mockGetUseCase;
    private readonly Mock<ILogger<NotificationsController>> _mockLogger;
    private readonly NotificationsController _controller;

    public NotificationsControllerTests()
    {
        _mockSendUseCase = new Mock<ISendNotificationUseCase>();
        _mockGetUseCase = new Mock<IGetUserNotificationsUseCase>();
        _mockLogger = new Mock<ILogger<NotificationsController>>();
        _controller = new NotificationsController(
            _mockSendUseCase.Object,
            _mockGetUseCase.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task SendNotification_WithValidRequest_ShouldReturnOk()
    {
        // Arrange
        var request = new NotificationRequestDto
        {
            UserId = "user123",
            Email = "test@example.com",
            Message = "Test message",
            Type = NotificationType.General
        };

        var response = new NotificationResponseDto
        {
            Success = true,
            Message = "Notificação enviada com sucesso",
            NotificationId = "notif-123"
        };

        _mockSendUseCase.Setup(x => x.ExecuteAsync(request))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.SendNotification(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task SendNotification_WhenUseCaseFails_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new NotificationRequestDto
        {
            UserId = "user123",
            Email = "test@example.com",
            Message = "Test message",
            Type = NotificationType.General
        };

        var response = new NotificationResponseDto
        {
            Success = false,
            Message = "Falha ao enviar notificação"
        };

        _mockSendUseCase.Setup(x => x.ExecuteAsync(request))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.SendNotification(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task SendNotification_WithInvalidModelState_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new NotificationRequestDto { Type = NotificationType.General };
        _controller.ModelState.AddModelError("UserId", "UserId é obrigatório");

        // Act
        var result = await _controller.SendNotification(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        _mockSendUseCase.Verify(x => x.ExecuteAsync(It.IsAny<NotificationRequestDto>()), Times.Never);
    }

    [Theory]
    [InlineData(NotificationType.VideoProcessingStarted)]
    [InlineData(NotificationType.VideoProcessingCompleted)]
    [InlineData(NotificationType.VideoProcessingFailed)]
    [InlineData(NotificationType.General)]
    public async Task SendNotification_WithDifferentTypes_ShouldWork(NotificationType type)
    {
        // Arrange
        var request = new NotificationRequestDto
        {
            UserId = "user123",
            Email = "test@example.com",
            Message = "Test",
            Type = type
        };

        var response = new NotificationResponseDto { Success = true };
        _mockSendUseCase.Setup(x => x.ExecuteAsync(It.IsAny<NotificationRequestDto>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.SendNotification(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task SendNotification_ShouldCallUseCaseWithCorrectParameters()
    {
        // Arrange
        var request = new NotificationRequestDto
        {
            UserId = "user123",
            Email = "test@example.com",
            Message = "Test message",
            Type = NotificationType.General
        };

        var response = new NotificationResponseDto { Success = true };
        _mockSendUseCase.Setup(x => x.ExecuteAsync(request))
            .ReturnsAsync(response);

        // Act
        await _controller.SendNotification(request);

        // Assert
        _mockSendUseCase.Verify(x => x.ExecuteAsync(
            It.Is<NotificationRequestDto>(r =>
                r.UserId == request.UserId &&
                r.Email == request.Email &&
                r.Message == request.Message &&
                r.Type == request.Type)), Times.Once);
    }

    [Fact]
    public async Task SendNotification_ShouldLogInformation()
    {
        // Arrange
        var request = new NotificationRequestDto
        {
            UserId = "user123",
            Email = "test@example.com",
            Message = "Test",
            Type = NotificationType.General
        };

        var response = new NotificationResponseDto { Success = true };
        _mockSendUseCase.Setup(x => x.ExecuteAsync(request))
            .ReturnsAsync(response);

        // Act
        await _controller.SendNotification(request);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Recebida requisição")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
