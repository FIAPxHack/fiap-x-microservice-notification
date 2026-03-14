using FluentAssertions;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Enums;
using NotificationService.Infrastructure.Persistence;
using Xunit;

namespace NotificationService.Tests.Infrastructure.Persistence;

/// <summary>
/// Testes unitários para o repositório InMemory de notificações
/// </summary>
public class InMemoryNotificationRepositoryTests
{
    [Fact]
    public async Task AddAsync_ShouldAddNotificationToRepository()
    {
        // Arrange
        var repository = new InMemoryNotificationRepository();
        var notification = new NotificationHistory("user1", "test@example.com", "Test Subject", "Test", NotificationType.General);

        // Act
        await repository.SaveAsync(notification);
        var result = await repository.GetByUserIdAsync("user1");

        // Assert
        result.Should().ContainSingle();
        result.First().Should().BeEquivalentTo(notification);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithNoNotifications_ShouldReturnEmptyList()
    {
        // Arrange
        var repository = new InMemoryNotificationRepository();

        // Act
        var result = await repository.GetByUserIdAsync("non-existent-user");

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByUserIdAsync_WithMultipleNotifications_ShouldReturnOnlyUserNotifications()
    {
        // Arrange
        var repository = new InMemoryNotificationRepository();
        var user1Notif1 = new NotificationHistory("user1", "email1@test.com", "Test Subject", "Message 1", NotificationType.General);
        var user1Notif2 = new NotificationHistory("user1", "email2@test.com", "Test Subject", "Message 2", NotificationType.General);
        var user2Notif = new NotificationHistory("user2", "email3@test.com", "Test Subject", "Message 3", NotificationType.General);

        await repository.SaveAsync(user1Notif1);
        await repository.SaveAsync(user1Notif2);
        await repository.SaveAsync(user2Notif);

        // Act
        var result = await repository.GetByUserIdAsync("user1");

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(n => n.UserId.Should().Be("user1"));
    }

    [Fact]
    public async Task AddAsync_CalledMultipleTimes_ShouldAddAllNotifications()
    {
        // Arrange
        var repository = new InMemoryNotificationRepository();
        var notifications = Enumerable.Range(1, 10)
            .Select(i => new NotificationHistory($"user{i}", $"email{i}@test.com", "Subject", $"Message {i}", NotificationType.General))
            .ToList();

        // Act
        foreach (var notification in notifications)
        {
            await repository.SaveAsync(notification);
        }

        // Assert
        for (int i = 1; i <= 10; i++)
        {
            var result = await repository.GetByUserIdAsync($"user{i}");
            result.Should().ContainSingle();
        }
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnNotificationsInOriginalOrder()
    {
        // Arrange
        var repository = new InMemoryNotificationRepository();
        var notif1 = new NotificationHistory("user1", "email1@test.com", "Test Subject", "First", NotificationType.General);
        var notif2 = new NotificationHistory("user1", "email2@test.com", "Test Subject", "Second", NotificationType.General);
        var notif3 = new NotificationHistory("user1", "email3@test.com", "Test Subject", "Third", NotificationType.General);

        await repository.SaveAsync(notif1);
        await repository.SaveAsync(notif2);
        await repository.SaveAsync(notif3);

        // Act
        var result = (await repository.GetByUserIdAsync("user1")).ToList();

        // Assert
        result[0].Message.Should().Be("First");
        result[1].Message.Should().Be("Second");
        result[2].Message.Should().Be("Third");
    }

    [Fact]
    public async Task AddAsync_WithSameNotificationTwice_ShouldAddBothInstances()
    {
        // Arrange
        var repository = new InMemoryNotificationRepository();
        var notification = new NotificationHistory("user1", "test@example.com", "Test Subject", "Test", NotificationType.General);

        // Act
        await repository.SaveAsync(notification);
        await repository.SaveAsync(notification);

        var result = await repository.GetByUserIdAsync("user1");

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithDifferentStatuses_ShouldReturnAll()
    {
        // Arrange
        var repository = new InMemoryNotificationRepository();
        var pending = new NotificationHistory("user1", "email1@test.com", "Test Subject", "Pending", NotificationType.General);
        var sent = new NotificationHistory("user1", "email2@test.com", "Test Subject", "Sent", NotificationType.General);
        var failed = new NotificationHistory("user1", "email3@test.com", "Test Subject", "Failed", NotificationType.General);

        sent.MarkAsSent();
        failed.MarkAsFailed();

        await repository.SaveAsync(pending);
        await repository.SaveAsync(sent);
        await repository.SaveAsync(failed);

        // Act
        var result = await repository.GetByUserIdAsync("user1");

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(n => n.Status == NotificationStatus.Pending);
        result.Should().Contain(n => n.Status == NotificationStatus.Sent);
        result.Should().Contain(n => n.Status == NotificationStatus.Failed);
    }

    [Fact]
    public async Task Repository_ShouldBeThreadSafe()
    {
        // Arrange
        var repository = new InMemoryNotificationRepository();
        var tasks = Enumerable.Range(1, 100)
            .Select(i => Task.Run(async () =>
            {
                var notification = new NotificationHistory($"user{i % 10}", $"email{i}@test.com", "Subject", $"Message {i}", NotificationType.General);
                await repository.SaveAsync(notification);
            }))
            .ToList();

        // Act
        await Task.WhenAll(tasks);

        // Assert
        for (int i = 0; i < 10; i++)
        {
            var result = await repository.GetByUserIdAsync($"user{i}");
            result.Should().HaveCount(10);
        }
    }

    [Theory]
    [InlineData(NotificationType.VideoProcessingStarted)]
    [InlineData(NotificationType.VideoProcessingCompleted)]
    [InlineData(NotificationType.VideoProcessingFailed)]
    [InlineData(NotificationType.General)]
    public async Task AddAsync_WithDifferentTypes_ShouldStoreCorrectly(NotificationType type)
    {
        // Arrange
        var repository = new InMemoryNotificationRepository();
        var notification = new NotificationHistory("user1", "test@example.com", "Test Subject", "Test Message", type);

        // Act
        await repository.SaveAsync(notification);
        var result = await repository.GetByUserIdAsync("user1");

        // Assert
        result.First().Type.Should().Be(type);
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnImmutableCopy()
    {
        // Arrange
        var repository = new InMemoryNotificationRepository();
        var notification = new NotificationHistory("user1", "test@example.com", "Test Subject", "Original", NotificationType.General);
        await repository.SaveAsync(notification);

        // Act
        var result1 = await repository.GetByUserIdAsync("user1");
        var result2 = await repository.GetByUserIdAsync("user1");

        // Assert
        result1.Should().NotBeSameAs(result2);
        result1.Should().BeEquivalentTo(result2);
    }
}
