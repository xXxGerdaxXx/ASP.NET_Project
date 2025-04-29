using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Infrastructure.Entities;
using Infrastructure.Hubs;
using Infrastructure.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Infrastructure.DTOs;

namespace MainApp.Controllers;

[Authorize]
[Route("api/notification")]
[ApiController]
public class NotificationController(IHubContext<NotificationHub> notificationHub, INotificationService notificationService) : ControllerBase
{
    private readonly IHubContext<NotificationHub> _notificationHub = notificationHub;
    private readonly INotificationService _notificationService = notificationService;

    [HttpPost]
    public async Task<IActionResult> CreateNotification(NotificationDto dto)
    {
        var notificationEntity = new NotificationEntity
        {
            NotificationTypeId = dto.NotificationTypeId,
            Message = dto.Message,
            Icon = dto.Icon,
            NotificationTargetGroupId = dto.NotificationTargetGroupId,
            CreatedAt = DateTime.Now
        };

        await _notificationService.AddNotificationAsync(notificationEntity);
        return Ok(new { success = true });
    }


    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var targetGroupId = User.IsInRole("Admin") ? 1 : 2;
        var notifications = await _notificationService.GetNotificationsAsync(userId, targetGroupId);

        return Ok(notifications);
    }

  
    [HttpPost("dismiss/{id}")]
    public async Task<IActionResult> DismissNotification(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        await _notificationService.DismissNotificationAsync(id, userId);
        await _notificationHub.Clients.User(userId).SendAsync("DismissNotification", id);
        return Ok(new { success = true });
    }

}
