using FluentAssertions;
using NotificationService.Models;
using Xunit;

namespace NotificationService.Tests.Models;

/// <summary>
/// Testes para os modelos legados em NotificationService.Models
/// </summary>
public class LegacyModelsTests
{
    [Fact]
    public void NotificationRequest_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var request = new NotificationRequest();

        // Assert
        request.UserId.Should().Be(string.Empty);
        request.Email.Should().Be(string.Empty);
        request.Subject.Should().Be(string.Empty);
        request.Message.Should().Be(string.Empty);
        request.Type.Should().Be(default(NotificationType));
    }

    [Fact]
    public void NotificationRequest_ShouldSetProperties()
    {
        // Arrange & Act
        var request = new NotificationRequest
        {
            UserId = "user123",
            Email = "test@example.com",
            Subject = "Test",
            Message = "Message",
            Type = NotificationType.General
        };

        // Assert
        request.UserId.Should().Be("user123");
        request.Email.Should().Be("test@example.com");
        request.Subject.Should().Be("Test");
        request.Message.Should().Be("Message");
        request.Type.Should().Be(NotificationType.General);
    }

    [Fact]
    public void NotificationResponse_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var response = new NotificationResponse();

        // Assert
        response.Success.Should().BeFalse();
        response.Message.Should().Be(string.Empty);
        response.NotificationId.Should().BeNull();
        response.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void NotificationResponse_ShouldSetProperties()
    {
        // Arrange
        var timestamp = DateTime.UtcNow;

        // Act
        var response = new NotificationResponse
        {
            Success = true,
            Message = "Success",
            NotificationId = "id123",
            Timestamp = timestamp
        };

        // Assert
        response.Success.Should().BeTrue();
        response.Message.Should().Be("Success");
        response.NotificationId.Should().Be("id123");
        response.Timestamp.Should().Be(timestamp);
    }

    [Fact]
    public void NotificationHistory_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var history = new NotificationHistory();

        // Assert
        history.Id.Should().NotBeNullOrEmpty();
        history.UserId.Should().Be(string.Empty);
        history.Email.Should().Be(string.Empty);
        history.Subject.Should().Be(string.Empty);
        history.Type.Should().Be(default(NotificationType));
        history.Status.Should().Be(default(NotificationStatus));
        history.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        history.SentAt.Should().BeNull();
    }

    [Fact]
    public void NotificationHistory_ShouldSetProperties()
    {
        // Arrange
        var createdAt = DateTime.UtcNow;
        var sentAt = DateTime.UtcNow.AddMinutes(1);

        // Act
        var history = new NotificationHistory
        {
            Id = "id123",
            UserId = "user123",
            Email = "test@test.com",
            Subject = "Test",
            Type = NotificationType.General,
            Status = NotificationStatus.Sent,
            CreatedAt = createdAt,
            SentAt = sentAt
        };

        // Assert
        history.Id.Should().Be("id123");
        history.UserId.Should().Be("user123");
        history.Email.Should().Be("test@test.com");
        history.Subject.Should().Be("Test");
        history.Type.Should().Be(NotificationType.General);
        history.Status.Should().Be(NotificationStatus.Sent);
        history.CreatedAt.Should().Be(createdAt);
        history.SentAt.Should().Be(sentAt);
    }

    [Fact]
    public void NotificationHistory_ShouldGenerateUniqueIds()
    {
        // Arrange & Act
        var history1 = new NotificationHistory();
        var history2 = new NotificationHistory();

        // Assert
        history1.Id.Should().NotBe(history2.Id);
    }

    [Theory]
    [InlineData(NotificationType.VideoProcessingStarted)]
    [InlineData(NotificationType.VideoProcessingCompleted)]
    [InlineData(NotificationType.VideoProcessingFailed)]
    [InlineData(NotificationType.General)]
    public void NotificationType_ShouldSupportAllValues(NotificationType type)
    {
        // Arrange & Act
        var request = new NotificationRequest { Type = type };

        // Assert
        request.Type.Should().Be(type);
    }

    [Theory]
    [InlineData(NotificationStatus.Pending)]
    [InlineData(NotificationStatus.Sent)]
    [InlineData(NotificationStatus.Failed)]
    public void NotificationStatus_ShouldSupportAllValues(NotificationStatus status)
    {
        // Arrange & Act
        var history = new NotificationHistory { Status = status };

        // Assert
        history.Status.Should().Be(status);
    }

    [Fact]
    public void NotificationHistory_IdProperty_ShouldBeSettable()
    {
        // Arrange
        var customId = "custom-id-123";

        // Act
        var history = new NotificationHistory { Id = customId };

        // Assert
        history.Id.Should().Be(customId);
    }

    [Fact]
    public void NotificationHistory_AllProperties_ShouldBeSettable()
    {
        // Arrange & Act
        var history = new NotificationHistory
        {
            Id = "id1",
            UserId = "user1",
            Email = "email@test.com",
            Subject = "Subject",
            Type = NotificationType.VideoProcessingCompleted,
            Status = NotificationStatus.Sent,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            SentAt = DateTime.UtcNow
        };

        // Assert
        history.Id.Should().Be("id1");
        history.UserId.Should().Be("user1");
        history.Email.Should().Be("email@test.com");
        history.Subject.Should().Be("Subject");
        history.Type.Should().Be(NotificationType.VideoProcessingCompleted);
        history.Status.Should().Be(NotificationStatus.Sent);
        history.CreatedAt.Should().BeOnOrBefore(DateTime.UtcNow);
        history.SentAt.Should().NotBeNull();
    }

    [Fact]
    public void NotificationHistory_Subject_ShouldBeGetableAndSettable()
    {
        // Arrange
        var  history = new NotificationHistory();
        
        // Act
        history.Subject = "New Subject";

        // Assert
        history.Subject.Should().Be("New Subject");
    }

    [Fact]
    public void NotificationHistory_Type_ShouldBeGetableAndSettable()
    {
        // Arrange
        var history = new NotificationHistory();
        
        // Act
        history.Type = NotificationType.VideoProcessingStarted;

        // Assert
        history.Type.Should().Be(NotificationType.VideoProcessingStarted);
    }

    [Fact]
    public void NotificationHistory_CreatedAt_ShouldBeGetableAndSettable()
    {
        // Arrange
        var history = new NotificationHistory();
        var customTime = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        
        // Act
        history.CreatedAt = customTime;

        // Assert
        history.CreatedAt.Should().Be(customTime);
    }

    [Fact]
    public void NotificationHistory_SentAt_Nullable_ShouldBeSettableToNull()
    {
        // Arrange
        var history = new NotificationHistory
        {
            SentAt = DateTime.UtcNow
        };
        
        // Act
        history.SentAt = null;

        // Assert
        history.SentAt.Should().BeNull();
    }

    [Fact]
    public void NotificationHistory_SentAt_Nullable_ShouldBeSettableToValue()
    {
        // Arrange
        var history = new NotificationHistory();
        var sentTime = DateTime.UtcNow;
        
        // Act
        history.SentAt = sentTime;

        // Assert
        history.SentAt.Should().Be(sentTime);
    }

    [Fact]
    public void NotificationHistory_AllEnumStatuses_ShouldBeSettable()
    {
        // Arrange & Act
        var history1 = new NotificationHistory { Status = NotificationStatus.Pending };
        var history2 = new NotificationHistory { Status = NotificationStatus.Sent };
        var history3 = new NotificationHistory { Status = NotificationStatus.Failed };

        // Assert
        history1.Status.Should().Be(NotificationStatus.Pending);
        history2.Status.Should().Be(NotificationStatus.Sent);
        history3.Status.Should().Be(NotificationStatus.Failed);
    }

    [Fact]
    public void NotificationHistory_AllEnumTypes_ShouldBeSettable()
    {
        // Arrange & Act
        var history1 = new NotificationHistory { Type = NotificationType.VideoProcessingStarted };
        var history2 = new NotificationHistory { Type = NotificationType.VideoProcessingCompleted };
        var history3 = new NotificationHistory { Type = NotificationType.VideoProcessingFailed };
        var history4 = new NotificationHistory { Type = NotificationType.General };

        // Assert
        history1.Type.Should().Be(NotificationType.VideoProcessingStarted);
        history2.Type.Should().Be(NotificationType.VideoProcessingCompleted);
        history3.Type.Should().Be(NotificationType.VideoProcessingFailed);
        history4.Type.Should().Be(NotificationType.General);
    }
}
