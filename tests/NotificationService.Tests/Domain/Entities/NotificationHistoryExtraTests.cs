using FluentAssertions;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Enums;
using Xunit;

namespace NotificationService.Tests.Domain.Entities;

/// <summary>
/// Testes extras para cobertura da entidade NotificationHistory
/// </summary>
public class NotificationHistoryExtraTests
{
    [Fact]
    public void Constructor_ShouldInitializePendingStatus()
    {
        // Act
        var notification = new NotificationHistory(
            "user1", "test@example.com", "Subject", "Message", NotificationType.General);

        // Assert
        notification.Status.Should().Be(NotificationStatus.Pending);
    }

    [Fact]
    public void Constructor_ShouldSetCreatedAtToUtcNow()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var notification = new NotificationHistory(
            "user1", "test@example.com", "Subject", "Message", NotificationType.General);

        // Assert
        var after = DateTime.UtcNow;
        notification.CreatedAt.Should().BeOnOrAfter(before);
        notification.CreatedAt.Should().BeOnOrBefore(after);
    }

    [Fact]
    public void Constructor_ShouldGenerateUniqueId()
    {
        // Act
        var notification1 = new NotificationHistory(
            "user1", "test@example.com", "Subject", "Message", NotificationType.General);
        var notification2 = new NotificationHistory(
            "user1", "test@example.com", "Subject", "Message", NotificationType.General);

        // Assert
        notification1.Id.Should().NotBe(notification2.Id);
    }

    [Theory]
    [InlineData("test@example.com", true)]
    [InlineData("user@domain.com", true)]
    [InlineData("name.surname@company.co.uk", true)]
    [InlineData("invalid", false)]
    [InlineData("@domain.com", false)]
    [InlineData("user@", false)]
    [InlineData("", false)]
    public void IsValidEmail_ShouldValidateCorrectly(string email, bool expectedValid)
    {
        // Arrange
        var notification = new NotificationHistory(
            "user1", email, "Subject", "Message", NotificationType.General);

        // Act
        var isValid = notification.IsValidEmail();

        // Assert
        isValid.Should().Be(expectedValid);
    }

    [Fact]
    public void MarkAsSent_ShouldSetStatusToSent()
    {
        // Arrange
        var notification = new NotificationHistory(
            "user1", "test@example.com", "Subject", "Message", NotificationType.General);

        // Act
        notification.MarkAsSent();

        // Assert
        notification.Status.Should().Be(NotificationStatus.Sent);
    }

    [Fact]
    public void MarkAsSent_ShouldSetSentAt()
    {
        // Arrange
        var notification = new NotificationHistory(
            "user1", "test@example.com", "Subject", "Message", NotificationType.General);

        // Act
        notification.MarkAsSent();

        // Assert
        notification.SentAt.Should().NotBeNull();
        notification.SentAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void MarkAsFailed_ShouldSetStatusToFailed()
    {
        // Arrange
        var notification = new NotificationHistory(
            "user1", "test@example.com", "Subject", "Message", NotificationType.General);

        // Act
        notification.MarkAsFailed();

        // Assert
        notification.Status.Should().Be(NotificationStatus.Failed);
    }

    [Fact]
    public void MarkAsFailed_AfterMarkAsSent_ShouldChangeStatus()
    {
        // Arrange
        var notification = new NotificationHistory(
            "user1", "test@example.com", "Subject", "Message", NotificationType.General);
        notification.MarkAsSent();

        // Act
        notification.MarkAsFailed();

        // Assert
        notification.Status.Should().Be(NotificationStatus.Failed);
    }

    [Fact]
    public void Properties_ShouldMatchConstructorValues()
    {
        // Arrange
        var userId = "user123";
        var email = "test@example.com";
        var subject = "Test Subject";
        var message = "Test Message";
        var type = NotificationType.VideoProcessingCompleted;

        // Act
        var notification = new NotificationHistory(userId, email, subject, message, type);

        // Assert
        notification.UserId.Should().Be(userId);
        notification.Email.Should().Be(email);
        notification.Subject.Should().Be(subject);
        notification.Message.Should().Be(message);
        notification.Type.Should().Be(type);
    }

    [Fact]
    public void SentAt_InitiallyNull_AfterSent_HasValue()
    {
        // Arrange
        var notification = new NotificationHistory(
            "user1", "test@example.com", "Subject", "Message", NotificationType.General);

        // Assert - Initially null
        notification.SentAt.Should().BeNull();

        // Act
        notification.MarkAsSent();

        // Assert - Now has value
        notification.SentAt.Should().NotBeNull();
    }

    [Theory]
    [InlineData("user@example.com")]
    [InlineData("admin@company.org")]
    [InlineData("test.user@domain.co.uk")]
    public void IsValidEmail_WithValidEmails_ShouldReturnTrue(string email)
    {
        // Arrange
        var notification = new NotificationHistory(
            "user1", email, "Subject", "Message", NotificationType.General);

        // Act & Assert
        notification.IsValidEmail().Should().BeTrue();
    }

    [Theory]
    [InlineData("plaintext")]
    [InlineData("missing@domain")]
    [InlineData("@nodomain.com")]
    public void IsValidEmail_WithInvalidEmails_ShouldReturnFalse(string email)
    {
        // Arrange
        var notification = new NotificationHistory(
            "user1", email, "Subject", "Message", NotificationType.General);

        // Act & Assert
        notification.IsValidEmail().Should().BeFalse();
    }
}
