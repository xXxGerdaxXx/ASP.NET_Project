using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Infrastructure.Entities;
using Infrastructure.Hubs;
using Infrastructure.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace MainApp.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class NotificationController(IHubContext<NotificationHub> notificationHub, INotificationService notificationService) : ControllerBase
{
    private readonly IHubContext<NotificationHub> _notificationHub = notificationHub;
    private readonly INotificationService _notificationService = notificationService;

    [HttpPost]
    public async Task<IActionResult> CreateNotification(NotificationEntity notificationEntity)
    {
        await _notificationService.AddNotificationAsync(notificationEntity); 
        return Ok(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)?? "anonymous";
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var notifications = await _notificationService.GetNotificationsAsync(userId);
        return Ok(notifications);
    }

    [HttpPost("dismiss/{id}")]
    public async Task<IActionResult> DismissNotification(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        await _notificationService.DismissNotificationAsync(id, userId);
        await _notificationHub.Clients.All.SendAsync("NotificationDismissed", id);
        return Ok(new { success = true });
    }
}
