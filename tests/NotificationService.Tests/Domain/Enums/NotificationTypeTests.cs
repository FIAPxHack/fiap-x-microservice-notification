using FluentAssertions;
using NotificationService.Domain.Enums;
using Xunit;

namespace NotificationService.Tests.Domain.Enums;

/// <summary>
/// Testes unitários para o enum NotificationType
/// </summary>
public class NotificationTypeTests
{
    [Fact]
    public void NotificationType_ShouldHaveVideoProcessingStarted()
    {
        // Arrange & Act
        var type = NotificationType.VideoProcessingStarted;

        // Assert
        type.Should().BeDefined();
        Enum.IsDefined(typeof(NotificationType), type).Should().BeTrue();
    }

    [Fact]
    public void NotificationType_ShouldHaveVideoProcessingCompleted()
    {
        // Arrange & Act
        var type = NotificationType.VideoProcessingCompleted;

        // Assert
        type.Should().BeDefined();
        Enum.IsDefined(typeof(NotificationType), type).Should().BeTrue();
    }

    [Fact]
    public void NotificationType_ShouldHaveVideoProcessingFailed()
    {
        // Arrange & Act
        var type = NotificationType.VideoProcessingFailed;

        // Assert
        type.Should().BeDefined();
        Enum.IsDefined(typeof(NotificationType), type).Should().BeTrue();
    }

    [Fact]
    public void NotificationType_ShouldHaveGeneral()
    {
        // Arrange & Act
        var type = NotificationType.General;

        // Assert
        type.Should().BeDefined();
        Enum.IsDefined(typeof(NotificationType), type).Should().BeTrue();
    }

    [Fact]
    public void NotificationType_ShouldHaveExactlyFourValues()
    {
        // Arrange & Act
        var values = Enum.GetValues(typeof(NotificationType));

        // Assert
        values.Length.Should().Be(4);
    }

    [Fact]
    public void NotificationType_AllValuesShouldHaveUniqueNumericValues()
    {
        // Arrange & Act
        var values = Enum.GetValues(typeof(NotificationType))
            .Cast<NotificationType>()
            .Select(v => (int)v)
            .ToList();

        // Assert
        values.Should().OnlyHaveUniqueItems();
    }

    [Theory]
    [InlineData(NotificationType.VideoProcessingStarted)]
    [InlineData(NotificationType.VideoProcessingCompleted)]
    [InlineData(NotificationType.VideoProcessingFailed)]
    [InlineData(NotificationType.General)]
    public void NotificationType_ShouldConvertToString(NotificationType type)
    {
        // Act
        var stringValue = type.ToString();

        // Assert
        stringValue.Should().NotBeNullOrEmpty();
        Enum.Parse<NotificationType>(stringValue).Should().Be(type);
    }

    [Fact]
    public void NotificationType_ShouldParseFromString_VideoProcessingStarted()
    {
        // Act
        var parsed = Enum.Parse<NotificationType>("VideoProcessingStarted");

        // Assert
        parsed.Should().Be(NotificationType.VideoProcessingStarted);
    }

    [Fact]
    public void NotificationType_ShouldParseFromString_VideoProcessingCompleted()
    {
        // Act
        var parsed = Enum.Parse<NotificationType>("VideoProcessingCompleted");

        // Assert
        parsed.Should().Be(NotificationType.VideoProcessingCompleted);
    }

    [Fact]
    public void NotificationType_ShouldParseFromString_VideoProcessingFailed()
    {
        // Act
        var parsed = Enum.Parse<NotificationType>("VideoProcessingFailed");

        // Assert
        parsed.Should().Be(NotificationType.VideoProcessingFailed);
    }

    [Fact]
    public void NotificationType_ShouldParseFromString_General()
    {
        // Act
        var parsed = Enum.Parse<NotificationType>("General");

        // Assert
        parsed.Should().Be(NotificationType.General);
    }

    [Fact]
    public void NotificationType_ShouldGetAllNames()
    {
        // Act
        var names = Enum.GetNames(typeof(NotificationType));

        // Assert
        names.Should().Contain("VideoProcessingStarted");
        names.Should().Contain("VideoProcessingCompleted");
        names.Should().Contain("VideoProcessingFailed");
        names.Should().Contain("General");
        names.Length.Should().Be(4);
    }

    [Fact]
    public void NotificationType_ShouldSupportComparison()
    {
        // Arrange
        var type1 = NotificationType.General;
        var type2 = NotificationType.General;
        var type3 = NotificationType.VideoProcessingStarted;

        // Assert
        type1.Should().Be(type2);
        type1.Should().NotBe(type3);
    }

    [Fact]
    public void NotificationType_DefaultValue_ShouldBeFirst()
    {
        // Arrange & Act
        var defaultValue = default(NotificationType);
        var firstValue = (NotificationType)0;

        // Assert
        defaultValue.Should().Be(firstValue);
    }
}
