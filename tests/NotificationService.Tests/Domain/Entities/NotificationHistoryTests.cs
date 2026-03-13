using FluentAssertions;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Enums;
using Xunit;

namespace NotificationService.Tests.Domain.Entities;

/// <summary>
/// Testes unitários para a entidade NotificationHistory
/// </summary>
public class NotificationHistoryTests
{
    [Fact]
    public void Constructor_ShouldCreateNotificationWithCorrectProperties()
    {
        // Arrange
        var userId = "test-user";
        var email = "test@example.com";
        var message = "Test message";
        var type = NotificationType.General;

        // Act
        var notification = new NotificationHistory(userId, email, message, type);

        // Assert
        notification.UserId.Should().Be(userId);
        notification.Email.Should().Be(email);
        notification.Message.Should().Be(message);
        notification.Type.Should().Be(type);
        notification.Status.Should().Be(NotificationStatus.Pending);
        notification.Id.Should().NotBeNullOrEmpty();
        notification.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        notification.SentAt.Should().BeNull();
    }

    [Fact]
    public void Constructor_ShouldGenerateUniqueIds()
    {
        // Arrange & Act
        var notification1 = new NotificationHistory("user1", "email1@test.com", "msg1", NotificationType.General);
        var notification2 = new NotificationHistory("user2", "email2@test.com", "msg2", NotificationType.General);

        // Assert
        notification1.Id.Should().NotBe(notification2.Id);
    }

    [Fact]
    public void MarkAsSent_ShouldUpdateStatusAndSetSentAt()
    {
        // Arrange
        var notification = new NotificationHistory("user", "email@test.com", "message", NotificationType.General);
        var beforeMark = DateTime.UtcNow;

        // Act
        notification.MarkAsSent();

        // Assert
        notification.Status.Should().Be(NotificationStatus.Sent);
        notification.SentAt.Should().NotBeNull();
        notification.SentAt.Value.Should().BeOnOrAfter(beforeMark);
        notification.SentAt.Value.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void MarkAsFailed_ShouldUpdateStatusToFailed()
    {
        // Arrange
        var notification = new NotificationHistory("user", "email@test.com", "message", NotificationType.General);

        // Act
        notification.MarkAsFailed();

        // Assert
        notification.Status.Should().Be(NotificationStatus.Failed);
        notification.SentAt.Should().BeNull();
    }

    [Fact]
    public void MarkAsSent_MultipleTimesCallings_ShouldKeepFirstSentAtValue()
    {
        // Arrange
        var notification = new NotificationHistory("user", "email@test.com", "message", NotificationType.General);
        
        // Act
        notification.MarkAsSent();
        var firstSentAt = notification.SentAt;
        Thread.Sleep(10);
        notification.MarkAsSent();

        // Assert
        notification.SentAt.Should().Be(firstSentAt);
    }

    [Theory]
    [InlineData(NotificationType.VideoProcessingStarted)]
    [InlineData(NotificationType.VideoProcessingCompleted)]
    [InlineData(NotificationType.VideoProcessingFailed)]
    [InlineData(NotificationType.General)]
    public void Constructor_ShouldAcceptAllNotificationTypes(NotificationType type)
    {
        // Act
        var notification = new NotificationHistory("user", "email@test.com", "message", type);

        // Assert
        notification.Type.Should().Be(type);
    }

    [Fact]
    public void Email_ShouldStoreExactValue()
    {
        // Arrange
        var email = "complex.email+tag@subdomain.example.co.uk";

        // Act
        var notification = new NotificationHistory("user", email, "message", NotificationType.General);

        // Assert
        notification.Email.Should().Be(email);
    }

    [Fact]
    public void Message_ShouldStoreCompleteMessage()
    {
        // Arrange
        var longMessage = new string('A', 1000);

        // Act
        var notification = new NotificationHistory("user", "email@test.com", longMessage, NotificationType.General);

        // Assert
        notification.Message.Should().Be(longMessage);
        notification.Message.Length.Should().Be(1000);
    }

    [Fact]
    public void PendingNotification_ShouldNotHaveSentAt()
    {
        // Arrange & Act
        var notification = new NotificationHistory("user", "email@test.com", "message", NotificationType.General);

        // Assert
        notification.Status.Should().Be(NotificationStatus.Pending);
        notification.SentAt.Should().BeNull();
    }
}
