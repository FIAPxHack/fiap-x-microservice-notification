using FluentAssertions;
using NotificationService.Application.DTOs;
using Xunit;

namespace NotificationService.Tests.Application.DTOs;

/// <summary>
/// Testes unitários para NotificationResponseDto
/// </summary>
public class NotificationResponseDtoTests
{
    [Fact]
    public void NotificationResponseDto_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var dto = new NotificationResponseDto();

        // Assert
        dto.Success.Should().BeFalse();
        dto.Message.Should().Be(string.Empty);
        dto.NotificationId.Should().BeNull();
        dto.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void NotificationResponseDto_ShouldSetPropertiesCorrectly_ForSuccessCase()
    {
        // Arrange
        var notificationId = Guid.NewGuid().ToString();
        var message = "Notification sent successfully";
        var timestamp = DateTime.UtcNow;

        // Act
        var dto = new NotificationResponseDto
        {
            Success = true,
            Message = message,
            NotificationId = notificationId,
            Timestamp = timestamp
        };

        // Assert
        dto.Success.Should().BeTrue();
        dto.Message.Should().Be(message);
        dto.NotificationId.Should().Be(notificationId);
        dto.Timestamp.Should().Be(timestamp);
    }

    [Fact]
    public void NotificationResponseDto_ShouldSetPropertiesCorrectly_ForFailureCase()
    {
        // Arrange
        var message = "Failed to send notification";
        var timestamp = DateTime.UtcNow;

        // Act
        var dto = new NotificationResponseDto
        {
            Success = false,
            Message = message,
            NotificationId = null,
            Timestamp = timestamp
        };

        // Assert
        dto.Success.Should().BeFalse();
        dto.Message.Should().Be(message);
        dto.NotificationId.Should().BeNull();
        dto.Timestamp.Should().Be(timestamp);
    }

    [Fact]
    public void NotificationResponseDto_NotificationIdProperty_ShouldBeNullable()
    {
        // Arrange & Act
        var dto = new NotificationResponseDto
        {
            Success = false,
            Message = "Error occurred"
        };

        // Assert
        dto.NotificationId.Should().BeNull();
    }

    [Fact]
    public void NotificationResponseDto_ShouldAllowCustomTimestamp()
    {
        // Arrange
        var customTimestamp = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        // Act
        var dto = new NotificationResponseDto
        {
            Timestamp = customTimestamp
        };

        // Assert
        dto.Timestamp.Should().Be(customTimestamp);
    }

    [Theory]
    [InlineData(true, "Success message", "id-123")]
    [InlineData(false, "Failure message", null)]
    [InlineData(true, "", "id-456")]
    public void NotificationResponseDto_ShouldHandleVariousCombinations(bool success, string message, string? notificationId)
    {
        // Arrange & Act
        var dto = new NotificationResponseDto
        {
            Success = success,
            Message = message,
            NotificationId = notificationId
        };

        // Assert
        dto.Success.Should().Be(success);
        dto.Message.Should().Be(message);
        dto.NotificationId.Should().Be(notificationId);
    }

    [Fact]
    public void NotificationResponseDto_MultipleInstances_ShouldHaveDifferentTimestamps()
    {
        // Arrange & Act
        var dto1 = new NotificationResponseDto();
        Thread.Sleep(10);
        var dto2 = new NotificationResponseDto();

        // Assert
        dto2.Timestamp.Should().BeAfter(dto1.Timestamp);
    }
}
