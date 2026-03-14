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
/// Testes adicionais para aumentar cobertura do NotificationsController
/// </summary>
public class NotificationsControllerAdditionalTests
{
    private readonly Mock<ISendNotificationUseCase> _mockSendUseCase;
    private readonly Mock<ILogger<NotificationsController>> _mockLogger;
    private readonly NotificationsController _controller;

    public NotificationsControllerAdditionalTests()
    {
        _mockSendUseCase = new Mock<ISendNotificationUseCase>();
        _mockLogger = new Mock<ILogger<NotificationsController>>();
        _controller = new NotificationsController(
            _mockSendUseCase.Object,
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
            Message = "Notificação enviada",
            NotificationId = Guid.NewGuid().ToString()
        };

        _mockSendUseCase.Setup(x => x.ExecuteAsync(It.IsAny<NotificationRequestDto>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.SendNotification(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be(response);
    }

    [Fact]
    public async Task SendNotification_WithFailedResponse_ShouldReturnBadRequest()
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
            Message = "Falha ao enviar"
        };

        _mockSendUseCase.Setup(x => x.ExecuteAsync(It.IsAny<NotificationRequestDto>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.SendNotification(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Health_ShouldReturnOkWithHealthInfo()
    {
        // Act
        var result = _controller.Health();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();
    }

    [Theory]
    [InlineData(NotificationType.General)]
    [InlineData(NotificationType.VideoProcessingStarted)]
    [InlineData(NotificationType.VideoProcessingCompleted)]
    [InlineData(NotificationType.VideoProcessingFailed)]
    public async Task SendNotification_WithDifferentTypes_ShouldCallUseCase(NotificationType type)
    {
        // Arrange
        var request = new NotificationRequestDto
        {
            UserId = "user123",
            Email = "test@example.com",
            Message = "Test message",
            Type = type
        };

        var response = new NotificationResponseDto
        {
            Success = true,
            Message = "Success"
        };

        _mockSendUseCase.Setup(x => x.ExecuteAsync(It.IsAny<NotificationRequestDto>()))
            .ReturnsAsync(response);

        // Act
        await _controller.SendNotification(request);

        // Assert
        _mockSendUseCase.Verify(x => x.ExecuteAsync(
            It.Is<NotificationRequestDto>(r => r.Type == type)), Times.Once);
    }

    [Fact]
    public async Task SendNotification_ShouldPassCorrectRequestToUseCase()
    {
        // Arrange
        var request = new NotificationRequestDto
        {
            UserId = "testuser",
            Email = "test@example.com",
            Subject = "Test Subject",
            Message = "Test Message",
            Type = NotificationType.General
        };

        var response = new NotificationResponseDto { Success = true };
        _mockSendUseCase.Setup(x => x.ExecuteAsync(It.IsAny<NotificationRequestDto>()))
            .ReturnsAsync(response);

        // Act
        await _controller.SendNotification(request);

        // Assert
        _mockSendUseCase.Verify(x => x.ExecuteAsync(
            It.Is<NotificationRequestDto>(r =>
                r.UserId == request.UserId &&
                r.Email == request.Email &&
                r.Subject == request.Subject &&
                r.Message == request.Message &&
                r.Type == request.Type)), Times.Once);
    }
}
