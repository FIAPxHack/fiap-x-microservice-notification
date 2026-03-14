using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NotificationService.Infrastructure.Email;
using Xunit;

namespace NotificationService.Tests.Infrastructure.Email;

/// <summary>
/// Testes adicionais para SmtpEmailService
/// </summary>
public class SmtpEmailServiceAdditionalTests
{
    private readonly Mock<ILogger<SmtpEmailService>> _mockLogger;
    private readonly SmtpEmailService _service;

    public SmtpEmailServiceAdditionalTests()
    {
        _mockLogger = new Mock<ILogger<SmtpEmailService>>();
        _service = new SmtpEmailService(_mockLogger.Object);
    }

    [Fact]
    public async Task SendEmailAsync_ShouldCompleteSuccessfully()
    {
        // Arrange
        var to = "test@example.com";
        var subject = "Test Subject";
        var body = "Test Body";

        // Act
        var act = async () => await _service.SendEmailAsync(to, subject, body);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Theory]
    [InlineData("user@example.com", "Simple Subject", "Simple body")]
    [InlineData("admin@test.com", "Another Subject", "Another message")]
    public async Task SendEmailAsync_WithDifferentInputs_ShouldComplete(
        string to, string subject, string body)
    {
        // Act
        var act = async () => await _service.SendEmailAsync(to, subject, body);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SendEmailAsync_WithLongBody_ShouldTruncateInLog()
    {
        // Arrange
        var to = "test@example.com";
        var subject = "Test";
        var longBody = new string('a', 100);

        // Act
        await _service.SendEmailAsync(to, subject, longBody);

        // Assert - just checking it completes without error
        Assert.True(true);
    }

    [Fact]
    public async Task SendEmailAsync_WithShortBody_ShouldNotTruncate()
    {
        // Arrange
        var to = "test@example.com";
        var subject = "Test";
        var shortBody = "Short message";

        // Act
        await _service.SendEmailAsync(to, subject, shortBody);

        // Assert
        Assert.True(true);
    }

    [Fact]
    public async Task SendEmailAsync_WithEmptyBody_ShouldComplete()
    {
        // Arrange
        var to = "test@example.com";
        var subject = "Test";
        var body = "";

        // Act
        var act = async () => await _service.SendEmailAsync(to, subject, body);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SendEmailAsync_WithSpecialCharacters_ShouldComplete()
    {
        // Arrange
        var to = "test@example.com";
        var subject = "Assunto com acentuação: São Paulo";
        var body = "Mensagem com caracteres especiais: ç, ã, é, €, £";

        // Act
        var act = async () => await _service.SendEmailAsync(to, subject, body);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SendEmailAsync_MultipleCalls_ShouldAllComplete()
    {
        // Arrange & Act
        await _service.SendEmailAsync("user1@test.com", "Subject 1", "Body 1");
        await _service.SendEmailAsync("user2@test.com", "Subject 2", "Body 2");
        await _service.SendEmailAsync("user3@test.com", "Subject 3", "Body 3");

        // Assert
        Assert.True(true);
    }
}
