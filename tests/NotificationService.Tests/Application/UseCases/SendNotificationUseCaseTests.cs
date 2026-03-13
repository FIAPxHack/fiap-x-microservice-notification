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
/// Testes unitários para o caso de uso SendNotificationUseCase
/// </summary>
public class SendNotificationUseCaseTests
{
    private readonly Mock<INotificationRepository> _mockRepository;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<ILogger<SendNotificationUseCase>> _mockLogger;
    private readonly SendNotificationUseCase _useCase;

    public SendNotificationUseCaseTests()
    {
        _mockRepository = new Mock<INotificationRepository>();
        _mockEmailService = new Mock<IEmailService>();
        _mockLogger = new Mock<ILogger<SendNotificationUseCase>>();
        _useCase = new SendNotificationUseCase(
            _mockRepository.Object,
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
            .ReturnsAsync(true);

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

        _mockRepository.Verify(x => x.AddAsync(
            It.Is<NotificationHistory>(n =>
                n.UserId == request.UserId &&
                n.Email == request.Email &&
                n.Message == request.Message &&
                n.Type == request.Type &&
                n.Status == NotificationStatus.Sent)), Times.Once);
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
            .ReturnsAsync(false);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Falha ao enviar notificação");

        _mockRepository.Verify(x => x.AddAsync(
            It.Is<NotificationHistory>(n => n.Status == NotificationStatus.Failed)), Times.Once);
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
        await act.Should().ThrowAsync<NotificationException>()
            .WithMessage("Erro ao processar notificação: SMTP error");

        _mockRepository.Verify(x => x.AddAsync(
            It.Is<NotificationHistory>(n => n.Status == NotificationStatus.Failed)), Times.Once);
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
            .ReturnsAsync(true);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        _mockRepository.Verify(x => x.AddAsync(
            It.Is<NotificationHistory>(n => n.Type == type)), Times.Once);
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
            .ReturnsAsync(true);

        // Act
        await _useCase.ExecuteAsync(request);

        // Assert
        capturedSubject.Should().Contain("Processamento Iniciado");
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
            .ReturnsAsync(true);

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
            .ReturnsAsync(true);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        _mockRepository.Verify(x => x.AddAsync(
            It.Is<NotificationHistory>(n => n.Message.Length == 5000)), Times.Once);
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
        _mockRepository.Setup(x => x.AddAsync(It.IsAny<NotificationHistory>()))
            .Callback<NotificationHistory>(n => savedNotification = n)
            .Returns(Task.CompletedTask);

        _mockEmailService.Setup(x => x.SendEmailAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.NotificationId.Should().Be(savedNotification!.Id);
    }
}
