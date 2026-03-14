using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NotificationService.Application.UseCases;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Interfaces.Repositories;
using Xunit;

namespace NotificationService.Tests.Application.UseCases;

/// <summary>
/// Testes unitários para o caso de uso GetUserNotificationsUseCase
/// </summary>
public class GetUserNotificationsUseCaseTests
{
    private readonly Mock<INotificationRepository> _mockRepository;
    private readonly Mock<ILogger<GetUserNotificationsUseCase>> _mockLogger;
    private readonly GetUserNotificationsUseCase _useCase;

    public GetUserNotificationsUseCaseTests()
    {
        _mockRepository = new Mock<INotificationRepository>();
        _mockLogger = new Mock<ILogger<GetUserNotificationsUseCase>>();
        _useCase = new GetUserNotificationsUseCase(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingNotifications_ShouldReturnAllUserNotifications()
    {
        // Arrange
        var userId = "user123";
        var notifications = new List<NotificationHistory>
        {
            new NotificationHistory(userId, "email1@test.com", "Test Subject", "Message 1", NotificationType.General),
            new NotificationHistory(userId, "email2@test.com", "Test Subject", "Message 2", NotificationType.VideoProcessingStarted),
            new NotificationHistory(userId, "email3@test.com", "Test Subject", "Message 3", NotificationType.VideoProcessingCompleted)
        };

        _mockRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(notifications);

        // Act
        var result = await _useCase.ExecuteAsync(userId);

        // Assert
        result.Should().HaveCount(3);
        result.Should().AllSatisfy(dto =>
        {
            dto.UserId.Should().Be(userId);
            dto.Email.Should().NotBeNullOrEmpty();
            dto.Message.Should().NotBeNullOrEmpty();
        });
    }

    [Fact]
    public async Task ExecuteAsync_WithNoNotifications_ShouldReturnEmptyList()
    {
        // Arrange
        var userId = "user-with-no-notifications";
        _mockRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(new List<NotificationHistory>());

        // Act
        var result = await _useCase.ExecuteAsync(userId);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldMapAllPropertiesCorrectly()
    {
        // Arrange
        var userId = "user123";
        var notification = new NotificationHistory(userId, "test@example.com", "Test Subject", "Test message", NotificationType.VideoProcessingCompleted);
        notification.MarkAsSent();

        _mockRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(new List<NotificationHistory> { notification });

        // Act
        var result = await _useCase.ExecuteAsync(userId);

        // Assert
        var dto = result.First();
        dto.Id.Should().Be(notification.Id);
        dto.UserId.Should().Be(notification.UserId);
        dto.Email.Should().Be(notification.Email);
        dto.Message.Should().Be(notification.Message);
        dto.Type.Should().Be(notification.Type);
        dto.Status.Should().Be(notification.Status);
        dto.CreatedAt.Should().Be(notification.CreatedAt);
        dto.SentAt.Should().Be(notification.SentAt);
    }

    [Fact]
    public async Task ExecuteAsync_WithMixedStatuses_ShouldReturnAll()
    {
        // Arrange
        var userId = "user123";
        var notifications = new List<NotificationHistory>
        {
            new NotificationHistory(userId, "email1@test.com", "Test Subject", "Pending", NotificationType.General),
            new NotificationHistory(userId, "email2@test.com", "Test Subject", "Sent", NotificationType.General),
            new NotificationHistory(userId, "email3@test.com", "Test Subject", "Failed", NotificationType.General)
        };

        notifications[1].MarkAsSent();
        notifications[2].MarkAsFailed();

        _mockRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(notifications);

        // Act
        var result = await _useCase.ExecuteAsync(userId);

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(dto => dto.Status == NotificationStatus.Pending);
        result.Should().Contain(dto => dto.Status == NotificationStatus.Sent);
        result.Should().Contain(dto => dto.Status == NotificationStatus.Failed);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCallRepositoryOnce()
    {
        // Arrange
        var userId = "user123";
        _mockRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(new List<NotificationHistory>());

        // Act
        await _useCase.ExecuteAsync(userId);

        // Assert
        _mockRepository.Verify(x => x.GetByUserIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithLargeResultSet_ShouldReturnAll()
    {
        // Arrange
        var userId = "user123";
        var notifications = Enumerable.Range(1, 100)
            .Select(i => new NotificationHistory(userId, $"email{i}@test.com", "Subject", $"Message {i}", NotificationType.General))
            .ToList();

        _mockRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(notifications);

        // Act
        var result = await _useCase.ExecuteAsync(userId);

        // Assert
        result.Should().HaveCount(100);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldLogInformation()
    {
        // Arrange
        var userId = "user123";
        _mockRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(new List<NotificationHistory>());

        // Act
        await _useCase.ExecuteAsync(userId);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Buscando notificações")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData("user1")]
    [InlineData("user2")]
    [InlineData("user-with-special-chars-123")]
    public async Task ExecuteAsync_WithDifferentUserIds_ShouldWorkCorrectly(string userId)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(new List<NotificationHistory>());

        // Act
        var result = await _useCase.ExecuteAsync(userId);

        // Assert
        result.Should().NotBeNull();
        _mockRepository.Verify(x => x.GetByUserIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldPreserveDateTimeValues()
    {
        // Arrange
        var userId = "user123";
        var specificDate = new DateTime(2026, 3, 10, 10, 30, 0, DateTimeKind.Utc);
        var notification = new NotificationHistory(userId, "test@example.com", "Test Subject", "Test", NotificationType.General);
        
        // Use reflection to set CreatedAt for testing
        var createdAtProperty = typeof(NotificationHistory).GetProperty(nameof(NotificationHistory.CreatedAt));
        createdAtProperty!.SetValue(notification, specificDate);

        _mockRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(new List<NotificationHistory> { notification });

        // Act
        var result = await _useCase.ExecuteAsync(userId);

        // Assert
        result.First().CreatedAt.Should().Be(specificDate);
    }
}
