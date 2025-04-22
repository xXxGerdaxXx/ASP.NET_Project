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
        var notifications = await _notificationService.GetNotificationsAsync(userId);
        return PartialView("_NotificationDropdown", notifications);
    }

}
