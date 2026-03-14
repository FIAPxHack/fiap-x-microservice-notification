using FluentAssertions;
using NotificationService.Application.DTOs;
using NotificationService.Domain.Enums;
using Xunit;

namespace NotificationService.Tests.Application.DTOs;

/// <summary>
/// Testes unitários para NotificationHistoryDto
/// </summary>
public class NotificationHistoryDtoTests
{
    [Fact]
    public void NotificationHistoryDto_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var dto = new NotificationHistoryDto();

        // Assert
        dto.Id.Should().Be(string.Empty);
        dto.UserId.Should().Be(string.Empty);
        dto.Email.Should().Be(string.Empty);
        dto.Subject.Should().Be(string.Empty);
        dto.Message.Should().Be(string.Empty);
        dto.Type.Should().Be(default(NotificationType));
        dto.Status.Should().Be(default(NotificationStatus));
        dto.CreatedAt.Should().Be(default(DateTime));
        dto.SentAt.Should().BeNull();
    }

    [Fact]
    public void NotificationHistoryDto_ShouldSetAllPropertiesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var userId = "user123";
        var email = "test@example.com";
        var subject = "Test Subject";
        var message = "Test Message";
        var type = NotificationType.VideoProcessingCompleted;
        var status = NotificationStatus.Sent;
        var createdAt = DateTime.UtcNow.AddHours(-1);
        var sentAt = DateTime.UtcNow;

        // Act
        var dto = new NotificationHistoryDto
        {
            Id = id,
            UserId = userId,
            Email = email,
            Subject = subject,
            Message = message,
            Type = type,
            Status = status,
            CreatedAt = createdAt,
            SentAt = sentAt
        };

        // Assert
        dto.Id.Should().Be(id);
        dto.UserId.Should().Be(userId);
        dto.Email.Should().Be(email);
        dto.Subject.Should().Be(subject);
        dto.Message.Should().Be(message);
        dto.Type.Should().Be(type);
        dto.Status.Should().Be(status);
        dto.CreatedAt.Should().Be(createdAt);
        dto.SentAt.Should().Be(sentAt);
    }

    [Theory]
    [InlineData(NotificationType.VideoProcessingStarted, NotificationStatus.Pending)]
    [InlineData(NotificationType.VideoProcessingCompleted, NotificationStatus.Sent)]
    [InlineData(NotificationType.VideoProcessingFailed, NotificationStatus.Failed)]
    [InlineData(NotificationType.General, NotificationStatus.Sent)]
    public void NotificationHistoryDto_ShouldHandleVariousTypeAndStatusCombinations(
        NotificationType type, 
        NotificationStatus status)
    {
        // Arrange & Act
        var dto = new NotificationHistoryDto
        {
            Type = type,
            Status = status
        };

        // Assert
        dto.Type.Should().Be(type);
        dto.Status.Should().Be(status);
    }

    [Fact]
    public void NotificationHistoryDto_SentAtProperty_ShouldBeNullable()
    {
        // Arrange & Act
        var dto = new NotificationHistoryDto
        {
            Id = "123",
            Status = NotificationStatus.Pending
        };

        // Assert
        dto.SentAt.Should().BeNull();
    }

    [Fact]
    public void NotificationHistoryDto_ShouldRepresentPendingNotification()
    {
        // Arrange & Act
        var dto = new NotificationHistoryDto
        {
            Id = "notif-123",
            UserId = "user-456",
            Email = "user@test.com",
            Subject = "Video Processing",
            Message = "Your video is being processed",
            Type = NotificationType.VideoProcessingStarted,
            Status = NotificationStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            SentAt = null
        };

        // Assert
        dto.Status.Should().Be(NotificationStatus.Pending);
        dto.SentAt.Should().BeNull();
    }

    [Fact]
    public void NotificationHistoryDto_ShouldRepresentSentNotification()
    {
        // Arrange
        var createdAt = DateTime.UtcNow.AddMinutes(-5);
        var sentAt = DateTime.UtcNow;

        // Act
        var dto = new NotificationHistoryDto
        {
            Id = "notif-123",
            UserId = "user-456",
            Email = "user@test.com",
            Subject = "Video Ready",
            Message = "Your video is ready",
            Type = NotificationType.VideoProcessingCompleted,
            Status = NotificationStatus.Sent,
            CreatedAt = createdAt,
            SentAt = sentAt
        };

        // Assert
        dto.Status.Should().Be(NotificationStatus.Sent);
        dto.SentAt.Should().NotBeNull();
        dto.SentAt.Should().BeAfter(dto.CreatedAt);
    }

    [Fact]
    public void NotificationHistoryDto_ShouldRepresentFailedNotification()
    {
        // Arrange & Act
        var dto = new NotificationHistoryDto
        {
            Id = "notif-789",
            UserId = "user-999",
            Email = "invalid@test.com",
            Subject = "Processing Failed",
            Message = "Video processing failed",
            Type = NotificationType.VideoProcessingFailed,
            Status = NotificationStatus.Failed,
            CreatedAt = DateTime.UtcNow,
            SentAt = null
        };

        // Assert
        dto.Status.Should().Be(NotificationStatus.Failed);
        dto.SentAt.Should().BeNull();
    }

    [Fact]
    public void NotificationHistoryDto_MultipleInstances_ShouldBeIndependent()
    {
        // Arrange & Act
        var dto1 = new NotificationHistoryDto
        {
            Id = "id1",
            UserId = "user1",
            Email = "user1@test.com"
        };

        var dto2 = new NotificationHistoryDto
        {
            Id = "id2",
            UserId = "user2",
            Email = "user2@test.com"
        };

        // Assert
        dto1.Id.Should().NotBe(dto2.Id);
        dto1.UserId.Should().NotBe(dto2.UserId);
        dto1.Email.Should().NotBe(dto2.Email);
    }

    [Fact]
    public void NotificationHistoryDto_ShouldAllowEmptyStrings()
    {
        // Arrange & Act
        var dto = new NotificationHistoryDto
        {
            Id = "",
            UserId = "",
            Email = "",
            Subject = "",
            Message = ""
        };

        // Assert
        dto.Id.Should().Be(string.Empty);
        dto.UserId.Should().Be(string.Empty);
        dto.Email.Should().Be(string.Empty);
        dto.Subject.Should().Be(string.Empty);
        dto.Message.Should().Be(string.Empty);
    }
}
