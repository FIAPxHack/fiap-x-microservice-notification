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
    private readonly NotificationsController _controller;

    public LegacyNotificationsControllerTests()
    {
        _notificationServiceMock = new Mock<INotificationService>();
        _controller = new NotificationsController(_notificationServiceMock.Object);
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
