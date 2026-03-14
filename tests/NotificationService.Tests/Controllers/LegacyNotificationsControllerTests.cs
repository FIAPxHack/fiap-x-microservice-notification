using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NotificationService.Controllers;
using NotificationService.Models;
using NotificationService.Services;
using Xunit;

namespace NotificationService.Tests.Controllers;

/// <summary>
/// Testes para o controller legado NotificationsController
/// </summary>
public class LegacyNotificationsControllerTests
{
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<ILogger<NotificationsController>> _loggerMock;
    private readonly NotificationsController _controller;

    public LegacyNotificationsControllerTests()
    {
        _notificationServiceMock = new Mock<INotificationService>();
        _loggerMock = new Mock<ILogger<NotificationsController>>();
        _controller = new NotificationsController(_notificationServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task SendNotification_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        var request = new NotificationRequest
        {
            UserId = "user123",
            Email = "test@example.com",
            Subject = "Test",
            Message = "Message",
            Type = NotificationType.General
        };

        var response = new NotificationResponse
        {
            Success = true,
            Message = "Success",
            NotificationId = "notif123"
        };

        _notificationServiceMock
            .Setup(x => x.SendNotificationAsync(It.IsAny<NotificationRequest>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.SendNotification(request);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedResponse = okResult.Value.Should().BeOfType<NotificationResponse>().Subject;
        returnedResponse.Success.Should().BeTrue();
        returnedResponse.NotificationId.Should().Be("notif123");
    }

    [Fact]
    public async Task SendNotification_ShouldReturnBadRequest_WhenServiceFails()
    {
        // Arrange
        var request = new NotificationRequest
        {
            UserId = "user123",
            Email = "test@example.com",
            Subject = "Test",
            Message = "Message",
            Type = NotificationType.General
        };

        var response = new NotificationResponse
        {
            Success = false,
            Message = "Error occurred"
        };

        _notificationServiceMock
            .Setup(x => x.SendNotificationAsync(It.IsAny<NotificationRequest>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.SendNotification(request);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var returnedResponse = badRequestResult.Value.Should().BeOfType<NotificationResponse>().Subject;
        returnedResponse.Success.Should().BeFalse();
    }

    [Fact]
    public async Task SendNotification_ShouldReturnBadRequest_WhenModelStateInvalid()
    {
        // Arrange
        var request = new NotificationRequest();
        _controller.ModelState.AddModelError("Email", "Email is required");

        // Act
        var result = await _controller.SendNotification(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetUserNotifications_ShouldReturnOk_WithNotifications()
    {
        // Arrange
        var userId = "user123";
        var notifications = new List<NotificationHistory>
        {
            new NotificationHistory
            {
                Id = "1",
                UserId = userId,
                Email = "test@test.com",
                Subject = "Test",
                Type = NotificationType.General,
                Status = NotificationStatus.Sent
            }
        };

        _notificationServiceMock
            .Setup(x => x.GetUserNotificationsAsync(userId))
            .ReturnsAsync(notifications);

        // Act
        var result = await _controller.GetUserNotifications(userId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedNotifications = okResult.Value.Should().BeAssignableTo<IEnumerable<NotificationHistory>>().Subject;
        returnedNotifications.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetUserNotifications_ShouldReturnBadRequest_WhenUserIdIsEmpty()
    {
        // Act
        var result = await _controller.GetUserNotifications("");

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetUserNotifications_ShouldReturnBadRequest_WhenUserIdIsWhitespace()
    {
        // Act
        var result = await _controller.GetUserNotifications("   ");

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetUserNotifications_ShouldReturnOk_WhenNoNotifications()
    {
        // Arrange
        var userId = "user123";
        _notificationServiceMock
            .Setup(x => x.GetUserNotificationsAsync(userId))
            .ReturnsAsync(new List<NotificationHistory>());

        // Act
        var result = await _controller.GetUserNotifications(userId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedNotifications = okResult.Value.Should().BeAssignableTo<IEnumerable<NotificationHistory>>().Subject;
        returnedNotifications.Should().BeEmpty();
    }

    [Fact]
    public void Health_ShouldReturnOk_WithHealthStatus()
    {
        // Act
        var result = _controller.Health();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().NotBeNull();
    }

    [Theory]
    [InlineData(NotificationType.VideoProcessingStarted)]
    [InlineData(NotificationType.VideoProcessingCompleted)]
    [InlineData(NotificationType.VideoProcessingFailed)]
    [InlineData(NotificationType.General)]
    public async Task SendNotification_ShouldHandleAllNotificationTypes(NotificationType type)
    {
        // Arrange
        var request = new NotificationRequest
        {
            UserId = "user123",
            Email = "test@example.com",
            Subject = "Test",
            Message = "Message",
            Type = type
        };

        var response = new NotificationResponse
        {
            Success = true,
            Message = "Success"
        };

        _notificationServiceMock
            .Setup(x => x.SendNotificationAsync(It.IsAny<NotificationRequest>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.SendNotification(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetUserNotifications_ShouldCallService_WithCorrectUserId()
    {
        // Arrange
        var userId = "specific-user-id";
        _notificationServiceMock
            .Setup(x => x.GetUserNotificationsAsync(userId))
            .ReturnsAsync(new List<NotificationHistory>());

        // Act
        await _controller.GetUserNotifications(userId);

        // Assert
        _notificationServiceMock.Verify(x => x.GetUserNotificationsAsync(userId), Times.Once);
    }

    [Fact]
    public async Task SendNotification_ShouldCallService_WithCorrectRequest()
    {
        // Arrange
        var request = new NotificationRequest
        {
            UserId = "user123",
            Email = "test@example.com",
            Subject = "Test",
            Message = "Message",
            Type = NotificationType.General
        };

        var response = new NotificationResponse { Success = true };

        _notificationServiceMock
            .Setup(x => x.SendNotificationAsync(request))
            .ReturnsAsync(response);

        // Act
        await _controller.SendNotification(request);

        // Assert
        _notificationServiceMock.Verify(x => x.SendNotificationAsync(request), Times.Once);
    }
}
