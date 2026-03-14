using FluentAssertions;
using NotificationService.Domain.Enums;
using Xunit;

namespace NotificationService.Tests.Domain.Enums;

/// <summary>
/// Testes unitários para o enum NotificationStatus
/// </summary>
public class NotificationStatusTests
{
    [Fact]
    public void NotificationStatus_ShouldHavePending()
    {
        // Arrange & Act
        var status = NotificationStatus.Pending;

        // Assert
        status.Should().BeDefined();
        Enum.IsDefined(typeof(NotificationStatus), status).Should().BeTrue();
    }

    [Fact]
    public void NotificationStatus_ShouldHaveSent()
    {
        // Arrange & Act
        var status = NotificationStatus.Sent;

        // Assert
        status.Should().BeDefined();
        Enum.IsDefined(typeof(NotificationStatus), status).Should().BeTrue();
    }

    [Fact]
    public void NotificationStatus_ShouldHaveFailed()
    {
        // Arrange & Act
        var status = NotificationStatus.Failed;

        // Assert
        status.Should().BeDefined();
        Enum.IsDefined(typeof(NotificationStatus), status).Should().BeTrue();
    }

    [Fact]
    public void NotificationStatus_ShouldHaveExactlyThreeValues()
    {
        // Arrange & Act
        var values = Enum.GetValues(typeof(NotificationStatus));

        // Assert
        values.Length.Should().Be(3);
    }

    [Fact]
    public void NotificationStatus_AllValuesShouldHaveUniqueNumericValues()
    {
        // Arrange & Act
        var values = Enum.GetValues(typeof(NotificationStatus))
            .Cast<NotificationStatus>()
            .Select(v => (int)v)
            .ToList();

        // Assert
        values.Should().OnlyHaveUniqueItems();
    }

    [Theory]
    [InlineData(NotificationStatus.Pending)]
    [InlineData(NotificationStatus.Sent)]
    [InlineData(NotificationStatus.Failed)]
    public void NotificationStatus_ShouldConvertToString(NotificationStatus status)
    {
        // Act
        var stringValue = status.ToString();

        // Assert
        stringValue.Should().NotBeNullOrEmpty();
        Enum.Parse<NotificationStatus>(stringValue).Should().Be(status);
    }

    [Fact]
    public void NotificationStatus_ShouldParseFromString_Pending()
    {
        // Act
        var parsed = Enum.Parse<NotificationStatus>("Pending");

        // Assert
        parsed.Should().Be(NotificationStatus.Pending);
    }

    [Fact]
    public void NotificationStatus_ShouldParseFromString_Sent()
    {
        // Act
        var parsed = Enum.Parse<NotificationStatus>("Sent");

        // Assert
        parsed.Should().Be(NotificationStatus.Sent);
    }

    [Fact]
    public void NotificationStatus_ShouldParseFromString_Failed()
    {
        // Act
        var parsed = Enum.Parse<NotificationStatus>("Failed");

        // Assert
        parsed.Should().Be(NotificationStatus.Failed);
    }

    [Fact]
    public void NotificationStatus_ShouldGetAllNames()
    {
        // Act
        var names = Enum.GetNames(typeof(NotificationStatus));

        // Assert
        names.Should().Contain("Pending");
        names.Should().Contain("Sent");
        names.Should().Contain("Failed");
        names.Length.Should().Be(3);
    }

    [Fact]
    public void NotificationStatus_ShouldSupportComparison()
    {
        // Arrange
        var status1 = NotificationStatus.Pending;
        var status2 = NotificationStatus.Pending;
        var status3 = NotificationStatus.Sent;

        // Assert
        status1.Should().Be(status2);
        status1.Should().NotBe(status3);
    }

    [Fact]
    public void NotificationStatus_DefaultValue_ShouldBeFirst()
    {
        // Arrange & Act
        var defaultValue = default(NotificationStatus);
        var firstValue = (NotificationStatus)0;

        // Assert
        defaultValue.Should().Be(firstValue);
    }

    [Fact]
    public void NotificationStatus_ShouldRepresentLifecycle()
    {
        // Arrange
        var pending = NotificationStatus.Pending;
        var sent = NotificationStatus.Sent;
        var failed = NotificationStatus.Failed;

        // Assert - Validate the status flow makes sense
        pending.Should().NotBe(sent);
        pending.Should().NotBe(failed);
        sent.Should().NotBe(failed);
    }

    [Fact]
    public void NotificationStatus_ShouldBeUsableInSwitch()
    {
        // Arrange
        var status = NotificationStatus.Sent;
        var result = string.Empty;

        // Act
        switch (status)
        {
            case NotificationStatus.Pending:
                result = "pending";
                break;
            case NotificationStatus.Sent:
                result = "sent";
                break;
            case NotificationStatus.Failed:
                result = "failed";
                break;
        }

        // Assert
        result.Should().Be("sent");
    }
}
