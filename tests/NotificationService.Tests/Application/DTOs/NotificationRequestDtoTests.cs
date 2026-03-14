using FluentAssertions;
using NotificationService.Application.DTOs;
using NotificationService.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace NotificationService.Tests.Application.DTOs;

/// <summary>
/// Testes unitários para NotificationRequestDto
/// </summary>
public class NotificationRequestDtoTests
{
    [Fact]
    public void NotificationRequestDto_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var dto = new NotificationRequestDto { Type = default };

        // Assert
        dto.UserId.Should().Be(string.Empty);
        dto.Email.Should().Be(string.Empty);
        dto.Message.Should().Be(string.Empty);
        dto.Subject.Should().BeNull();
        dto.Type.Should().Be(default(NotificationType));
    }

    [Fact]
    public void NotificationRequestDto_ShouldSetPropertiesCorrectly()
    {
        // Arrange & Act
        var dto = new NotificationRequestDto
        {
            UserId = "user123",
            Email = "test@example.com",
            Subject = "Test Subject",
            Message = "Test Message",
            Type = NotificationType.General
        };

        // Assert
        dto.UserId.Should().Be("user123");
        dto.Email.Should().Be("test@example.com");
        dto.Subject.Should().Be("Test Subject");
        dto.Message.Should().Be("Test Message");
        dto.Type.Should().Be(NotificationType.General);
    }

    [Theory]
    [InlineData(NotificationType.VideoProcessingStarted)]
    [InlineData(NotificationType.VideoProcessingCompleted)]
    [InlineData(NotificationType.VideoProcessingFailed)]
    [InlineData(NotificationType.General)]
    public void NotificationRequestDto_ShouldAcceptAllNotificationTypes(NotificationType type)
    {
        // Arrange & Act
        var dto = new NotificationRequestDto { Type = type };

        // Assert
        dto.Type.Should().Be(type);
    }

    [Fact]
    public void NotificationRequestDto_ShouldHaveRequiredAttribute_OnUserId()
    {
        // Arrange
        var property = typeof(NotificationRequestDto).GetProperty(nameof(NotificationRequestDto.UserId));

        // Act
        var attribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault() as RequiredAttribute;

        // Assert
        attribute.Should().NotBeNull();
        attribute?.ErrorMessage.Should().Be("UserId é obrigatório");
    }

    [Fact]
    public void NotificationRequestDto_ShouldHaveRequiredAttribute_OnEmail()
    {
        // Arrange
        var property = typeof(NotificationRequestDto).GetProperty(nameof(NotificationRequestDto.Email));

        // Act
        var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault() as RequiredAttribute;
        var emailAttribute = property?.GetCustomAttributes(typeof(EmailAddressAttribute), false).FirstOrDefault() as EmailAddressAttribute;

        // Assert
        requiredAttribute.Should().NotBeNull();
        requiredAttribute?.ErrorMessage.Should().Be("Email é obrigatório");
        emailAttribute.Should().NotBeNull();
        emailAttribute?.ErrorMessage.Should().Be("Email em formato inválido");
    }

    [Fact]
    public void NotificationRequestDto_ShouldHaveRequiredAttribute_OnMessage()
    {
        // Arrange
        var property = typeof(NotificationRequestDto).GetProperty(nameof(NotificationRequestDto.Message));

        // Act
        var attribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault() as RequiredAttribute;

        // Assert
        attribute.Should().NotBeNull();
        attribute?.ErrorMessage.Should().Be("Mensagem é obrigatória");
    }

    [Fact]
    public void NotificationRequestDto_ShouldHaveRequiredAttribute_OnType()
    {
        // Arrange
        var property = typeof(NotificationRequestDto).GetProperty(nameof(NotificationRequestDto.Type));

        // Act
        var attribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault() as RequiredAttribute;

        // Assert
        attribute.Should().NotBeNull();
    }

    [Fact]
    public void NotificationRequestDto_SubjectProperty_ShouldBeNullable()
    {
        // Arrange & Act
        var dto = new NotificationRequestDto
        {
            UserId = "user123",
            Email = "test@test.com",
            Message = "Test",
            Type = NotificationType.General
        };

        // Assert
        dto.Subject.Should().BeNull();
    }

    [Fact]
    public void NotificationRequestDto_ShouldValidateCorrectly_WhenAllRequiredFieldsProvided()
    {
        // Arrange
        var dto = new NotificationRequestDto
        {
            UserId = "user123",
            Email = "valid@email.com",
            Message = "Test Message",
            Type = NotificationType.General
        };

        // Act
        var validationContext = new ValidationContext(dto);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(dto, validationContext, validationResults, true);

        // Assert
        isValid.Should().BeTrue();
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void NotificationRequestDto_ShouldFail_WhenUserIdIsMissing()
    {
        // Arrange
        var dto = new NotificationRequestDto
        {
            UserId = "",
            Email = "valid@email.com",
            Message = "Test Message",
            Type = NotificationType.General
        };

        // Act
        var validationContext = new ValidationContext(dto);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(dto, validationContext, validationResults, true);

        // Assert
        isValid.Should().BeFalse();
        validationResults.Should().ContainSingle(v => v.ErrorMessage == "UserId é obrigatório");
    }

    [Fact]
    public void NotificationRequestDto_ShouldFail_WhenEmailIsInvalid()
    {
        // Arrange
        var dto = new NotificationRequestDto
        {
            UserId = "user123",
            Email = "invalid-email",
            Message = "Test Message",
            Type = NotificationType.General
        };

        // Act
        var validationContext = new ValidationContext(dto);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(dto, validationContext, validationResults, true);

        // Assert
        isValid.Should().BeFalse();
        validationResults.Should().Contain(v => v.ErrorMessage == "Email em formato inválido");
    }

    [Fact]
    public void NotificationRequestDto_ShouldFail_WhenMessageIsMissing()
    {
        // Arrange
        var dto = new NotificationRequestDto
        {
            UserId = "user123",
            Email = "valid@email.com",
            Message = "",
            Type = NotificationType.General
        };

        // Act
        var validationContext = new ValidationContext(dto);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(dto, validationContext, validationResults, true);

        // Assert
        isValid.Should().BeFalse();
        validationResults.Should().ContainSingle(v => v.ErrorMessage == "Mensagem é obrigatória");
    }
}
