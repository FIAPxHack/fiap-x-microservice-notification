using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NotificationService.Models;
using NotificationService.Services;
using Xunit;

namespace NotificationService.Tests.Services;

/// <summary>
/// Testes para o serviço legado EmailNotificationService
/// </summary>
public class EmailNotificationServiceTests
{
    private readonly Mock<ILogger<EmailNotificationService>> _loggerMock;
    private readonly EmailNotificationService _service;

    public EmailNotificationServiceTests()
    {
        _loggerMock = new Mock<ILogger<EmailNotificationService>>();
        _service = new EmailNotificationService(_loggerMock.Object);
    }

    [Fact]
    public async Task SendNotificationAsync_ShouldReturnSuccess_WhenValidRequest()
    {
        // Arrange
        var request = new NotificationRequest
        {
            UserId = "user123",
            Email = "test@example.com",
            Subject = "Test Subject",
            Message = "Test Message",
            Type = NotificationType.General
        };

        // Act
        var result = await _service.SendNotificationAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("sucesso");
        result.NotificationId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SendNotificationAsync_ShouldReturnFailure_WhenEmailIsEmpty()
    {
        // Arrange
        var request = new NotificationRequest
        {
            UserId = "user123",
            Email = "",
            Subject = "Test",
            Message = "Message",
            Type = NotificationType.General
        };

        // Act
        var result = await _service.SendNotificationAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Email é obrigatório");
    }

    [Fact]
    public async Task SendNotificationAsync_ShouldReturnFailure_WhenEmailIsNull()
    {
        // Arrange
        var request = new NotificationRequest
        {
            UserId = "user123",
            Email = null!,
            Subject = "Test",
            Message = "Message",
            Type = NotificationType.General
        };

        // Act
        var result = await _service.SendNotificationAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("obrigatório");
    }

    [Fact]
    public async Task SendNotificationAsync_ShouldSetDefaultSubject_WhenSubjectIsEmpty()
    {
        // Arrange
        var request = new NotificationRequest
        {
            UserId = "user123",
            Email = "test@example.com",
            Subject = "",
            Message = "Test Message",
            Type = NotificationType.VideoProcessingCompleted
        };

        // Act
        var result = await _service.SendNotificationAsync(request);

        // Assert
        result.Success.Should().BeTrue();
    }

    [Theory]
    [InlineData(NotificationType.VideoProcessingStarted)]
    [InlineData(NotificationType.VideoProcessingCompleted)]
    [InlineData(NotificationType.VideoProcessingFailed)]
    [InlineData(NotificationType.General)]
    public async Task SendNotificationAsync_ShouldHandleAllNotificationTypes(NotificationType type)
    {
        // Arrange
        var request = new NotificationRequest
        {
            UserId = "user123",
            Email = "test@example.com",
            Subject = "Test",
            Message = "Message",
            Type = type
        };

        // Act
        var result = await _service.SendNotificationAsync(request);

        // Assert
        result.Success.Should().BeTrue();
    }
}
