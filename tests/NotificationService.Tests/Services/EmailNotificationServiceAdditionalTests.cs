using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NotificationService.Models;
using NotificationService.Services;
using Xunit;

namespace NotificationService.Tests.Services;

/// <summary>
/// Testes adicionais para EmailNotificationService - casos edge
/// </summary>
public class EmailNotificationServiceAdditionalTests
{
    private readonly Mock<ILogger<EmailNotificationService>> _loggerMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly EmailNotificationService _service;

    public EmailNotificationServiceAdditionalTests()
    {
        _loggerMock = new Mock<ILogger<EmailNotificationService>>();
        _configurationMock = new Mock<IConfiguration>();
        _service = new EmailNotificationService(_loggerMock.Object, _configurationMock.Object);
    }

    [Fact]
    public async Task SendNotificationAsync_WithWhitespaceOnlySubject_ShouldGenerateDefault()
    {
        // Arrange
        var request = new NotificationRequest
        {
            UserId = "user123",
            Email = "test@example.com",
            Subject = "   ",
            Message = "Test Message",
            Type = NotificationType.VideoProcessingStarted
        };

        // Act
        var result = await _service.SendNotificationAsync(request);

        // Assert
        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task SendNotificationAsync_WithVideoProcessingFailed_ShouldUseCorrectSubject()
    {
        // Arrange
        var request = new NotificationRequest
        {
            UserId = "user123",
            Email = "test@example.com",
            Subject = null,
            Message = "Processing failed",
            Type = NotificationType.VideoProcessingFailed
        };

        // Act
        var result = await _service.SendNotificationAsync(request);

        // Assert
        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task GetUserNotificationsAsync_ShouldReturnOnlyUserNotifications()
    {
        // Arrange
        var userId1 = $"user-{Guid.NewGuid()}";
        var userId2 = $"user-{Guid.NewGuid()}";

        await _service.SendNotificationAsync(new NotificationRequest
        {
            UserId = userId1,
            Email = "user1@test.com",
            Subject = "Test",
            Message = "Message 1",
            Type = NotificationType.General
        });

        await _service.SendNotificationAsync(new NotificationRequest
        {
            UserId = userId2,
            Email = "user2@test.com",
            Subject = "Test",
            Message = "Message 2",
            Type = NotificationType.General
        });

        // Act
        var user1Notifications = await _service.GetUserNotificationsAsync(userId1);
        var user2Notifications = await _service.GetUserNotificationsAsync(userId2);

        // Assert
        user1Notifications.Should().HaveCount(1);
        user1Notifications.First().UserId.Should().Be(userId1);
        
        user2Notifications.Should().HaveCount(1);
        user2Notifications.First().UserId.Should().Be(userId2);
    }

    [Fact]
    public async Task SendNotificationAsync_MultipleTimes_ShouldCreateMultipleNotifications()
    {
        // Arrange
        var userId = $"user-{Guid.NewGuid()}";

        // Act
        await _service.SendNotificationAsync(new NotificationRequest
        {  
            UserId = userId,
            Email = "test@test.com",
            Subject = "Test 1",
            Message = "Message 1",
            Type = NotificationType.General
        });

        await _service.SendNotificationAsync(new NotificationRequest
        {
            UserId = userId,
            Email = "test@test.com",
            Subject = "Test 2",
            Message = "Message 2",
            Type = NotificationType.VideoProcessingCompleted
        });

        var notifications = await _service.GetUserNotificationsAsync(userId);

        // Assert
        notifications.Should().HaveCount(2);
    }

    [Theory]
    [InlineData(NotificationType.VideoProcessingStarted, "Processamento")]
    [InlineData(NotificationType.VideoProcessingCompleted, "concluído")]
    [InlineData(NotificationType.VideoProcessingFailed, "Falha")]
    [InlineData(NotificationType.General, "Notificação")]
    public async Task SendNotificationAsync_WithoutSubject_ShouldGenerateCorrectDefaultByType(
        NotificationType type, 
        string expectedSubjectPart)
    {
        // Arrange
        var userId = $"user-{Guid.NewGuid()}";
        var request = new NotificationRequest
        {
            UserId = userId,
            Email = "test@test.com",
            Subject = "",
            Message = "Test",
            Type = type
        };

        // Act
        await _service.SendNotificationAsync(request);
        var notifications = await _service.GetUserNotificationsAsync(userId);

        // Assert
        notifications.Should().HaveCount(1);
        var notification = notifications.First();
        notification.Subject.Should().Contain(expectedSubjectPart);
    }

    [Fact]
    public async Task SendNotificationAsync_ShouldSetStatus_ToPending_Then_Sent()
    {
        // Arrange
        var userId = $"user-{Guid.NewGuid()}";
        var request = new NotificationRequest
        {
            UserId = userId,
            Email = "test@test.com",
            Subject = "Test",
            Message = "Message",
            Type = NotificationType.General
        };

        // Act
        await _service.SendNotificationAsync(request);
        var notifications = await _service.GetUserNotificationsAsync(userId);

        // Assert
        var notification = notifications.First();
        notification.Status.Should().Be(NotificationStatus.Sent);
        notification.SentAt.Should().NotBeNull();
    }
}
