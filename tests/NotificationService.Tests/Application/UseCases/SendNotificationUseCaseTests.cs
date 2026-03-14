using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NotificationService.Application.DTOs;
using NotificationService.Application.UseCases;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Exceptions;
using NotificationService.Domain.Interfaces.Services;
using Xunit;

namespace NotificationService.Tests.Application.UseCases;

/// <summary>
/// Testes unitários para o caso de uso SendNotificationUseCase
/// </summary>
public class SendNotificationUseCaseTests
{
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<ILogger<SendNotificationUseCase>> _mockLogger;
    private readonly SendNotificationUseCase _useCase;

    public SendNotificationUseCaseTests()
    {
        _mockEmailService = new Mock<IEmailService>();
        _mockLogger = new Mock<ILogger<SendNotificationUseCase>>();
        _useCase = new SendNotificationUseCase(
            _mockEmailService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldSendEmailAndReturnSuccess()
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
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Notificação enviada com sucesso");
        result.NotificationId.Should().NotBeNullOrEmpty();

        _mockEmailService.Verify(x => x.SendEmailAsync(
            request.Email,
            It.IsAny<string>(),
            request.Message), Times.Once);

    }

    [Fact]
    public async Task ExecuteAsync_WhenEmailFails_ShouldMarkAsFailedAndReturnError()
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
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>()))
            .ThrowsAsync(new Exception("Email send failed"));

        // Act
        var act = async () => await _useCase.ExecuteAsync(request);

        // Assert
        await act.Should().ThrowAsync<NotificationException>();

    }

    [Fact]
    public async Task ExecuteAsync_WhenEmailServiceThrowsException_ShouldMarkAsFailedAndThrow()
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
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>()))
            .ThrowsAsync(new Exception("SMTP error"));

        // Act
        var act = async () => await _useCase.ExecuteAsync(request);

        // Assert
        await act.Should().ThrowAsync<NotificationException>();

    }

    [Theory]
    [InlineData(NotificationType.VideoProcessingStarted)]
    [InlineData(NotificationType.VideoProcessingCompleted)]
    [InlineData(NotificationType.VideoProcessingFailed)]
    [InlineData(NotificationType.General)]
    public async Task ExecuteAsync_WithDifferentTypes_ShouldProcessCorrectly(NotificationType type)
    {
        // Arrange
        var request = new NotificationRequestDto
        {
            UserId = "user123",
            Email = "test@example.com",
            Message = "Test message",
            Type = type
        };

        _mockEmailService.Setup(x => x.SendEmailAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_WithVideoProcessingStarted_ShouldUseCorrectSubject()
    {
        // Arrange
        var request = new NotificationRequestDto
        {
            UserId = "user123",
            Email = "test@example.com",
            Message = "Your video is being processed",
            Type = NotificationType.VideoProcessingStarted
        };

        string? capturedSubject = null;
        _mockEmailService.Setup(x => x.SendEmailAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>()))
            .Callback<string, string, string>((email, subject, message) => capturedSubject = subject)
            .Returns(Task.CompletedTask);

        // Act
        await _useCase.ExecuteAsync(request);

        // Assert
        capturedSubject.Should().Contain("Processamento");
        capturedSubject.Should().Contain("iniciado");
    }

    [Fact]
    public async Task ExecuteAsync_ShouldLogCorrectInformation()
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
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        await _useCase.ExecuteAsync(request);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Enviando notificação")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task ExecuteAsync_WithLongMessage_ShouldProcessSuccessfully()
    {
        // Arrange
        var longMessage = new string('A', 5000);
        var request = new NotificationRequestDto
        {
            UserId = "user123",
            Email = "test@example.com",
            Message = longMessage,
            Type = NotificationType.General
        };

        _mockEmailService.Setup(x => x.SendEmailAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnNotificationIdFromSavedEntity()
    {
        // Arrange
        var request = new NotificationRequestDto
        {
            UserId = "user123",
            Email = "test@example.com",
            Message = "Test message",
            Type = NotificationType.General
        };

        NotificationHistory? savedNotification = null;
        _mockRepository.Setup(x => x.SaveAsync(It.IsAny<NotificationHistory>()))
            .Callback<NotificationHistory>(n => savedNotification = n)
            .ReturnsAsync((NotificationHistory n) => n);

        _mockEmailService.Setup(x => x.SendEmailAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.NotificationId.Should().Be(savedNotification!.Id);
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
    }

    [Fact]
    public async Task ExecuteAsync_WithCustomSubject_ShouldUseProvidedSubject()
    {
        // Arrange
        var customSubject = "My Custom Subject";
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
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>()))
            .Callback<string, string, string>((email, subject, message) => capturedSubject = subject)
            .Returns(Task.CompletedTask);

        // Act
        await _useCase.ExecuteAsync(request);

        // Assert
        capturedSubject.Should().Be(customSubject);
    }

    [Fact]
    public async Task ExecuteAsync_WithNullSubject_ShouldGenerateDefault()
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
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>()))
            .Callback<string, string, string>((email, subject, message) => capturedSubject = subject)
            .Returns(Task.CompletedTask);

        // Act
        await _useCase.ExecuteAsync(request);

        // Assert
        capturedSubject.Should().NotBeNullOrEmpty();
        capturedSubject.Should().Contain("FIAP X");
    }

    [Theory]
    [InlineData("test@test", false)]
    [InlineData("test", false)]
    [InlineData("@test", false)]
    [InlineData("test@", false)]
    [InlineData("test@example.com", true)]
    public async Task ExecuteAsync_WithVariousEmailFormats_ShouldValidateCorrectly(string email, bool shouldSucceed)
    {
        // Arrange
        var request = new NotificationRequestDto
        {
            UserId = "user123",
            Email = email,
            Message = "Test message",
            Type = NotificationType.General
        };

        _mockEmailService.Setup(x => x.SendEmailAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Success.Should().Be(shouldSucceed);
    }
}
