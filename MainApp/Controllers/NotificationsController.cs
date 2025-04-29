using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MainApp.Controllers;

public class NotificationsController(INotificationService notificationService) : Controller
{
    private readonly INotificationService _notificationService = notificationService;

    public async Task<IActionResult> GetDropdown()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(); // Protect: only logged-in users can fetch notifications

        // Decide if Admin or User
        var targetGroupId = User.IsInRole("Admin") ? 1 : 2;

        var notifications = await _notificationService.GetNotificationsAsync(userId, targetGroupId);
        return PartialView("_NotificationDropdown", notifications);
    }
}

