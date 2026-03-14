using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NotificationService.Application.DTOs;
using NotificationService.Application.UseCases;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Exceptions;
using NotificationService.Domain.Interfaces.Repositories;
using NotificationService.Domain.Interfaces.Services;
using Xunit;

namespace NotificationService.Tests.Application.UseCases;

/// <summary>
/// Testes adicionais para aumentar cobertura do SendNotificationUseCase
/// </summary>
public class SendNotificationUseCaseAdditionalTests
{
    private readonly Mock<INotificationRepository> _mockRepository;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<ILogger<SendNotificationUseCase>> _mockLogger;
    private readonly SendNotificationUseCase _useCase;

    public SendNotificationUseCaseAdditionalTests()
    {
        _mockRepository = new Mock<INotificationRepository>();
        _mockEmailService = new Mock<IEmailService>();
        _mockLogger = new Mock<ILogger<SendNotificationUseCase>>();

        _mockRepository.Setup(x => x.SaveAsync(It.IsAny<NotificationHistory>()))
            .ReturnsAsync((NotificationHistory notification) => notification);
        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<NotificationHistory>()))
            .Returns(Task.CompletedTask);

        _useCase = new SendNotificationUseCase(
            _mockRepository.Object,
            _mockEmailService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidEmail_ShouldReturnFailure()
    {
        // Arrange
        var request = new NotificationRequestDto
        {
            UserId = "user123",
            Email = "invalid-email",
            Message = "Test message",
            Type = NotificationType.General
        };

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Email inválido");
        _mockEmailService.Verify(x => x.SendEmailAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData("notanemail")]
    [InlineData("@nodomain.com")]
    [InlineData("user@")]
    public async Task ExecuteAsync_WithVariousInvalidEmails_ShouldReturnFailure(string invalidEmail)
    {
        // Arrange
        var request = new NotificationRequestDto
        {
            UserId = "user123",
            Email = invalidEmail,
            Message = "Test message",
            Type = NotificationType.General
        };

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_WithValidEmailAndNoSubject_ShouldGenerateDefaultSubject()
    {
        // Arrange
        var request = new NotificationRequestDto
        {
            UserId = "user123",
            Email = "test@example.com",
            Subject = null,
            Message = "Test message",
            Type = NotificationType.General
        };

        string? capturedSubject = null;
        _mockEmailService.Setup(x => x.SendEmailAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Callback<string, string, string>((_, subject, _) => capturedSubject = subject)
            .Returns(Task.CompletedTask);

        // Act
        await _useCase.ExecuteAsync(request);

        // Assert
        capturedSubject.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task ExecuteAsync_WithCustomSubject_ShouldUseProvidedSubject()
    {
        // Arrange
        var customSubject = "Custom Subject Line";
        var request = new NotificationRequestDto
        {
            UserId = "user123",
            Email = "test@example.com",
            Subject = customSubject,
            Message = "Test message",
            Type = NotificationType.General
        };

        string? capturedSubject = null;
        _mockEmailService.Setup(x => x.SendEmailAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Callback<string, string, string>((_, subject, _) => capturedSubject = subject)
            .Returns(Task.CompletedTask);

        // Act
        await _useCase.ExecuteAsync(request);

        // Assert
        capturedSubject.Should().Be(customSubject);
    }

    [Theory]
    [InlineData(NotificationType.VideoProcessingStarted, "Processamento")]
    [InlineData(NotificationType.VideoProcessingCompleted, "concluído")]
    [InlineData(NotificationType.VideoProcessingFailed, "Falha")]
    [InlineData(NotificationType.General, "Notificação")]
    public async Task ExecuteAsync_WithDifferentTypes_ShouldGenerateCorrectSubject(
        NotificationType type, string expectedSubjectPart)
    {
        // Arrange
        var request = new NotificationRequestDto
        {
            UserId = "user123",
            Email = "test@example.com",
            Message = "Test message",
            Type = type
        };

        string? capturedSubject = null;
        _mockEmailService.Setup(x => x.SendEmailAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Callback<string, string, string>((_, subject, _) => capturedSubject = subject)
            .Returns(Task.CompletedTask);

        // Act
        await _useCase.ExecuteAsync(request);

        // Assert
        capturedSubject.Should().Contain(expectedSubjectPart);
    }

    [Fact]
    public async Task ExecuteAsync_WhenEmailServiceFails_ShouldLogError()
    {
        // Arrange
        var request = new NotificationRequestDto
        {
            UserId = "user123",
            Email = "test@example.com",
            Message = "Test message",
            Type = NotificationType.General
        };

        _mockEmailService.Setup(x => x.SendEmailAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("SMTP failure"));

        // Act
        var act = async () => await _useCase.ExecuteAsync(request);

        // Assert
        await act.Should().ThrowAsync<NotificationException>()
            .WithMessage("*Falha ao enviar notificação*");
    }

    [Fact]
    public async Task ExecuteAsync_WithWhitespaceSubject_ShouldGenerateDefaultSubject()
    {
        // Arrange
        var request = new NotificationRequestDto
        {
            UserId = "user123",
            Email = "test@example.com",
            Subject = "   ",
            Message = "Test message",
            Type = NotificationType.General
        };

        string? capturedSubject = null;
        _mockEmailService.Setup(x => x.SendEmailAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Callback<string, string, string>((_, subject, _) => capturedSubject = subject)
            .Returns(Task.CompletedTask);

        // Act
        await _useCase.ExecuteAsync(request);

        // Assert
        capturedSubject.Should().NotBeNullOrWhiteSpace();
        capturedSubject.Should().NotBe("   ");
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCallEmailServiceWithCorrectParameters()
    {
        // Arrange
        var request = new NotificationRequestDto
        {
            UserId = "user123",
            Email = "test@example.com",
            Subject = "Test Subject",
            Message = "Test message",
            Type = NotificationType.General
        };

        // Act
        await _useCase.ExecuteAsync(request);

        // Assert
        _mockEmailService.Verify(x => x.SendEmailAsync(
            request.Email,
            request.Subject,
            request.Message), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldReturnNotificationId()
    {
        // Arrange
        var request = new NotificationRequestDto
        {
            UserId = "user123",
            Email = "test@example.com",
            Message = "Test message",
            Type = NotificationType.General
        };

        _mockEmailService.Setup(x => x.SendEmailAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.NotificationId.Should().NotBeNullOrEmpty();
        Guid.TryParse(result.NotificationId, out _).Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_MultipleCalls_ShouldGenerateDifferentIds()
    {
        // Arrange
        var request = new NotificationRequestDto
        {
            UserId = "user123",
            Email = "test@example.com",
            Message = "Test message",
            Type = NotificationType.General
        };

        _mockEmailService.Setup(x => x.SendEmailAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var result1 = await _useCase.ExecuteAsync(request);
        var result2 = await _useCase.ExecuteAsync(request);

        // Assert
        result1.NotificationId.Should().NotBe(result2.NotificationId);
    }
}
