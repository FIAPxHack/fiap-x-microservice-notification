using FluentAssertions;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Enums;
using Xunit;

namespace NotificationService.Tests.Domain.Entities;

/// <summary>
/// Testes adicionais para NotificationHistory - validação completa
/// </summary>
public class NotificationHistoryAdditionalTests
{
    [Fact]
    public void IsValidEmail_ShouldReturnTrue_ForValidEmail()
    {
        // Arrange
        var notification = new NotificationHistory(
            "user123",
            "valid@email.com",
            "Test",
            "Message",
            NotificationType.General
        );

        // Act
        var result = notification.IsValidEmail();

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("@test")]
    [InlineData("test@")]
    [InlineData("   ")]
    [InlineData("")]
    public void IsValidEmail_ShouldReturnFalse_ForInvalidEmail(string email)
    {
        // Arrange
        var notification = new NotificationHistory(
            "user123",
            email,
            "Test",
            "Message",
            NotificationType.General
        );

        // Act
        var result = notification.IsValidEmail();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenUserIdIsNull()
    {
        // Act
        Action act = () => new NotificationHistory(
            null!,
            "test@test.com",
            "Test",
            "Message",
            NotificationType.General
        );

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("userId");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenEmailIsNull()
    {
        // Act
        Action act = () => new NotificationHistory(
            "user123",
            null!,
            "Test",
            "Message",
            NotificationType.General
        );

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("email");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenSubjectIsNull()
    {
        // Act
        Action act = () => new NotificationHistory(
            "user123",
            "test@test.com",
            null!,
            "Message",
            NotificationType.General
        );

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("subject");
    }

    [Fact]
    public void Constructor_ShouldAcceptNullMessage_AndSetEmpty()
    {
        // Act
        var notification = new NotificationHistory(
            "user123",
            "test@test.com",
            "Subject",
            null!,
            NotificationType.General
        );

        // Assert
        notification.Message.Should().Be(string.Empty);
    }

    [Fact]
    public void IsValidEmail_ShouldReturnTrue_WhenEmailHasMultipleDots()
    {
        // Arrange
        var notification = new NotificationHistory(
            "user123",
            "user@subdomain.domain.com",
            "Test",
            "Message",
            NotificationType.General
        );

        // Act
        var result = notification.IsValidEmail();

        // Assert
        result.Should().BeTrue();
    }
}
