using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NotificationService.Infrastructure.Email;
using Xunit;

namespace NotificationService.Tests.Infrastructure.Email;

/// <summary>
/// Testes unitários para o serviço de email SMTP
/// </summary>
public class SmtpEmailServiceTests
{
    private readonly Mock<ILogger<SmtpEmailService>> _mockLogger;
    private readonly SmtpEmailService _emailService;
    private readonly Mock<IConfiguration> _mockConfiguration;

    public SmtpEmailServiceTests()
    {
        _mockLogger = new Mock<ILogger<SmtpEmailService>>();
        _mockConfiguration = new Mock<IConfiguration>();
        _emailService = new SmtpEmailService(_mockLogger.Object, _mockConfiguration.Object);
    }

    [Fact]
    public async Task SendEmailAsync_WithValidParameters_ShouldComplete()
    {
        // Arrange
        var to = "test@example.com";
        var subject = "Test Subject";
        var body = "Test Body";

        // Act
        var act = async () => await _emailService.SendEmailAsync(to, subject, body);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SendEmailAsync_ShouldLogSuccess()
    {
        // Arrange
        var to = "test@example.com";
        var subject = "Test Subject";
        var body = "Test Body";

        // Act
        await _emailService.SendEmailAsync(to, subject, body);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Email enviado com sucesso")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData("simple@example.com", "Subject", "Body")]
    [InlineData("complex.email+tag@subdomain.example.com", "Long Subject Line", "Long body content")]
    [InlineData("user@domain.co.uk", "Special chars: áéíóú", "Body with\nnewlines")]
    public async Task SendEmailAsync_WithVariousInputs_ShouldSucceed(string to, string subject, string body)
    {
        // Act
        var act = async () => await _emailService.SendEmailAsync(to, subject, body);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SendEmailAsync_WithLongBody_ShouldHandleCorrectly()
    {
        // Arrange
        var to = "test@example.com";
        var subject = "Test";
        var body = new string('A', 10000);

        // Act
        var act = async () => await _emailService.SendEmailAsync(to, subject, body);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SendEmailAsync_WithHtmlContent_ShouldWork()
    {
        // Arrange
        var to = "test@example.com";
        var subject = "HTML Email";
        var body = "<html><body><h1>Test</h1><p>Content</p></body></html>";

        // Act
        var act = async () => await _emailService.SendEmailAsync(to, subject, body);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SendEmailAsync_MultipleSpaces_ShouldStillWork()
    {
        // Arrange
        var to = "test@example.com";
        var subject = "Test     with     spaces";
        var body = "Body     with     spaces";

        // Act
        var act = async () => await _emailService.SendEmailAsync(to, subject, body);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SendEmailAsync_CalledMultipleTimes_ShouldSucceedEachTime()
    {
        // Arrange
        var emails = new[]
        {
            ("test1@example.com", "Subject1", "Body1"),
            ("test2@example.com", "Subject2", "Body2"),
            ("test3@example.com", "Subject3", "Body3")
        };

        // Act & Assert
        foreach (var (to, subject, body) in emails)
        {
            var act = async () => await _emailService.SendEmailAsync(to, subject, body);
            await act.Should().NotThrowAsync();
        }
    }

    [Fact]
    public async Task SendEmailAsync_WithEmptySubject_ShouldStillWork()
    {
        // Arrange
        var to = "test@example.com";
        var subject = "";
        var body = "Test body";

        // Act
        var act = async () => await _emailService.SendEmailAsync(to, subject, body);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SendEmailAsync_WithEmptyBody_ShouldStillWork()
    {
        // Arrange
        var to = "test@example.com";
        var subject = "Test";
        var body = "";

        // Act
        var act = async () => await _emailService.SendEmailAsync(to, subject, body);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SendEmailAsync_ShouldSimulateEmailSending()
    {
        // Arrange
        var to = "test@example.com";
        var subject = "Simulation Test";
        var body = "This tests the simulation mode";

        // Act
        await _emailService.SendEmailAsync(to, subject, body);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("SIMULAÇÃO")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
