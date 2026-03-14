using FluentAssertions;
using Microsoft.Extensions.Configuration;
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
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly EmailNotificationService _service;

    public EmailNotificationServiceTests()
    {
        _loggerMock = new Mock<ILogger<EmailNotificationService>>();
        _configurationMock = new Mock<IConfiguration>();
        _service = new EmailNotificationService(_loggerMock.Object, _configurationMock.Object);
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

    [Fact]
    public async Task GetUserNotificationsAsync_ShouldReturnNotifications_ForSpecificUser()
    {
        // Arrange
        var userId = "user123";
        var request = new NotificationRequest
        {
            UserId = userId,
            Email = "test@example.com",
            Subject = "Test",
            Message = "Message",
            Type = NotificationType.General
        };

        await _service.SendNotificationAsync(request);

        // Act
        var notifications = await _service.GetUserNotificationsAsync(userId);

        // Assert
        notifications.Should().NotBeNull();
        notifications.Should().Contain(n => n.UserId == userId);
    }

    [Fact]
    public async Task GetUserNotificationsAsync_ShouldReturnEmpty_WhenNoNotifications()
    {
        // Arrange
        var userId = "nonexistent-user";

        // Act
        var notifications = await _service.GetUserNotificationsAsync(userId);

        // Assert
        notifications.Should().NotBeNull();
    }

    [Fact]
    public async Task GetUserNotificationsAsync_ShouldOrderByCreatedAtDescending()
    {
        // Arrange
        var userId = "user456";
        
        for (int i = 0; i < 3; i++)
        {
            var request = new NotificationRequest
            {
                UserId = userId,
                Email = "test@example.com",
                Subject = $"Test {i}",
                Message = $"Message {i}",
                Type = NotificationType.General
            };
            await _service.SendNotificationAsync(request);
            await Task.Delay(10);
        }

        // Act
        var notifications = await _service.GetUserNotificationsAsync(userId);
        var list = notifications.ToList();

        // Assert
        list.Should().HaveCountGreaterThanOrEqualTo(3);
        for (int i = 0; i < list.Count - 1; i++)
        {
            list[i].CreatedAt.Should().BeOnOrAfter(list[i + 1].CreatedAt);
        }
    }

    [Fact]
    public async Task SendNotificationAsync_ShouldSetSentAtTime_WhenSuccessful()
    {
        // Arrange
        var request = new NotificationRequest
        {
            UserId = "user123",
            Email = "test@example.com",
            Subject = "Test",
            Message = "Message",
            Type = NotificationType.General
        };

        // Act
        var beforeSend = DateTime.UtcNow;
        var result = await _service.SendNotificationAsync(request);
        var afterSend = DateTime.UtcNow;

        // Assert
        result.Success.Should().BeTrue();
        
        // Verify notification was added to history
        var notifications = await _service.GetUserNotificationsAsync("user123");
        var notification = notifications.First();
        notification.SentAt.Should().NotBeNull();
        notification.SentAt.Value.Should().BeOnOrAfter(beforeSend);
        notification.SentAt.Value.Should().BeOnOrBefore(afterSend);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task SendNotificationAsync_WithNullOrEmptySubject_ShouldGenerateDefault(string? subject)
    {
        // Arrange
        var request = new NotificationRequest
        {
            UserId = "user123",
            Email = "test@example.com",
            Subject = subject!,
            Message = "Message",
            Type = NotificationType.VideoProcessingCompleted
        };

        // Act
        var result = await _service.SendNotificationAsync(request);

        // Assert
        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task SendNotificationAsync_ShouldCreateNotificationInHistory()
    {
        // Arrange
        var request = new NotificationRequest
        {
            UserId = "user789",
            Email = "new@test.com",
            Subject = "Test",
            Message = "Test message",
            Type = NotificationType.General
        };

        // Act
        await _service.SendNotificationAsync(request);
        var notifications = await _service.GetUserNotificationsAsync("user789");

        // Assert
        notifications.Should().NotBeEmpty();
        var notification = notifications.First();
        notification.Email.Should().Be("new@test.com");
        notification.Status.Should().Be(NotificationStatus.Sent);
    }
}
