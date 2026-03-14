using FluentAssertions;
using NotificationService.Domain.Exceptions;
using Xunit;

namespace NotificationService.Tests.Domain.Exceptions;

/// <summary>
/// Testes unitários para as exceções de domínio
/// </summary>
public class NotificationExceptionTests
{
    [Fact]
    public void NotificationException_ShouldBeCreatedWithMessage()
    {
        // Arrange
        var message = "Test error message";

        // Act
        var exception = new NotificationException(message);

        // Assert
        exception.Message.Should().Be(message);
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void NotificationException_ShouldBeCreatedWithMessageAndInnerException()
    {
        // Arrange
        var message = "Test error message";
        var innerException = new InvalidOperationException("Inner error");

        // Act
        var exception = new NotificationException(message, innerException);

        // Assert
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
        exception.InnerException.Message.Should().Be("Inner error");
    }

    [Fact]
    public void NotificationException_ShouldInheritFromException()
    {
        // Arrange & Act
        var exception = new NotificationException("Test");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void InvalidEmailException_ShouldBeCreatedWithEmail()
    {
        // Arrange
        var email = "invalid@email";

        // Act
        var exception = new InvalidEmailException(email);

        // Assert
        exception.Message.Should().Contain(email);
        exception.Message.Should().Contain("Email inválido");
        exception.Should().BeAssignableTo<NotificationException>();
    }

    [Fact]
    public void InvalidEmailException_ShouldInheritFromNotificationException()
    {
        // Arrange & Act
        var exception = new InvalidEmailException("test@test.com");

        // Assert
        exception.Should().BeAssignableTo<NotificationException>();
        exception.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void EmailSendException_ShouldBeCreatedWithMessageAndInnerException()
    {
        // Arrange
        var message = "Failed to send email";
        var innerException = new Exception("SMTP error");

        // Act
        var exception = new EmailSendException(message, innerException);

        // Assert
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
        exception.Should().BeAssignableTo<NotificationException>();
    }

    [Fact]
    public void EmailSendException_ShouldInheritFromNotificationException()
    {
        // Arrange & Act
        var exception = new EmailSendException("Test", new Exception());

        // Assert
        exception.Should().BeAssignableTo<NotificationException>();
        exception.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void InvalidEmailException_MultipleInstances_ShouldContainDifferentEmails()
    {
        // Arrange & Act
        var exception1 = new InvalidEmailException("user1@invalid");
        var exception2 = new InvalidEmailException("user2@invalid");

        // Assert
        exception1.Message.Should().Contain("user1@invalid");
        exception2.Message.Should().Contain("user2@invalid");
        exception1.Message.Should().NotBe(exception2.Message);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("test@")]
    [InlineData("@test.com")]
    public void InvalidEmailException_ShouldHandleVariousInvalidEmails(string email)
    {
        // Act
        var exception = new InvalidEmailException(email);

        // Assert
        exception.Message.Should().Contain(email);
        exception.Message.Should().Contain("Email inválido");
    }

    [Fact]
    public void InvalidEmailException_ShouldHandleEmptyEmail()
    {
        // Arrange
        var email = "";

        // Act
        var exception = new InvalidEmailException(email);

        // Assert
        exception.Message.Should().Contain("Email inválido");
    }
}
