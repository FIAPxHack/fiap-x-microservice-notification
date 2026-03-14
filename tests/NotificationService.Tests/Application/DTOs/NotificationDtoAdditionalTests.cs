using FluentAssertions;
using NotificationService.Application.DTOs;
using NotificationService.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace NotificationService.Tests.Application.DTOs;

/// <summary>
/// Testes adicionais para DTOs de notificação
/// </summary>
public class NotificationDtoAdditionalTests
{
    [Fact]
    public void NotificationRequestDto_WithValidData_ShouldPassValidation()
    {
        // Arrange
        var dto = new NotificationRequestDto
        {
            UserId = "user123",
            Email = "test@example.com",
            Message = "Test message",
            Type = NotificationType.General
        };

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(dto, context, results, true);

        // Assert
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    [Fact]
    public void NotificationRequestDto_WithEmptyUserId_ShouldFailValidation()
    {
        // Arrange
        var dto = new NotificationRequestDto
        {
            UserId = "",
            Email = "test@example.com",
            Message = "Test message",
            Type = NotificationType.General
        };

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(dto, context, results, true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().Contain(r => r.ErrorMessage!.Contains("UserId"));
    }

    [Fact]
    public void NotificationRequestDto_WithEmptyEmail_ShouldFailValidation()
    {
        // Arrange
        var dto = new NotificationRequestDto
        {
            UserId = "user123",
            Email = "",
            Message = "Test message",
            Type = NotificationType.General
        };

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(dto, context, results, true);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void NotificationRequestDto_WithInvalidEmail_ShouldFailValidation()
    {
        // Arrange
        var dto = new NotificationRequestDto
        {
            UserId = "user123",
            Email = "not-an-email",
            Message = "Test message",
            Type = NotificationType.General
        };

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(dto, context, results, true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().Contain(r => r.ErrorMessage!.Contains("Email"));
    }

    [Fact]
    public void NotificationRequestDto_WithEmptyMessage_ShouldFailValidation()
    {
        // Arrange
        var dto = new NotificationRequestDto
        {
            UserId = "user123",
            Email = "test@example.com",
            Message = "",
            Type = NotificationType.General
        };

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(dto, context, results, true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().Contain(r => r.ErrorMessage!.Contains("Mensagem"));
    }

    [Fact]
    public void NotificationRequestDto_WithOptionalSubject_ShouldPassValidation()
    {
        // Arrange
        var dto = new NotificationRequestDto
        {
            UserId = "user123",
            Email = "test@example.com",
            Message = "Test message",
            Type = NotificationType.General,
            Subject = null
        };

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(dto, context, results, true);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void NotificationResponseDto_DefaultConstructor_ShouldSetTimestamp()
    {
        // Act
        var dto = new NotificationResponseDto();

        // Assert
        dto.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void NotificationResponseDto_ShouldSetProperties()
    {
        // Arrange & Act
        var dto = new NotificationResponseDto
        {
            Success = true,
            Message = "Success",
            NotificationId = "123"
        };

        // Assert
        dto.Success.Should().BeTrue();
        dto.Message.Should().Be("Success");
        dto.NotificationId.Should().Be("123");
    }

    [Theory]
    [InlineData(NotificationType.General)]
    [InlineData(NotificationType.VideoProcessingStarted)]
    [InlineData(NotificationType.VideoProcessingCompleted)]
    [InlineData(NotificationType.VideoProcessingFailed)]
    public void NotificationRequestDto_WithAllTypes_ShouldBeValid(NotificationType type)
    {
        // Arrange
        var dto = new NotificationRequestDto
        {
            UserId = "user123",
            Email = "test@example.com",
            Message = "Test message",
            Type = type
        };

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(dto, context, results, true);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void NotificationResponseDto_MultipleInstances_ShouldHaveUniqueTimestamps()
    {
        // Act
        var dto1 = new NotificationResponseDto();
        var dto2 = new NotificationResponseDto();

        // Assert
        dto1.Timestamp.Should().BeSameDateAs(dto2.Timestamp);
    }
}
