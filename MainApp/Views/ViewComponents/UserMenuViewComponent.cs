using Infrastructure.Entities;
using Infrastructure.Interfaces;
using MainApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MainApp.Views.ViewComponents;

public class UserMenuViewComponent : ViewComponent
{
    private readonly UserManager<UserEntity> _userManager;
    private readonly INotificationService _notificationService;

    public UserMenuViewComponent(UserManager<UserEntity> userManager, INotificationService notificationService)
    {
        _userManager = userManager;
        _notificationService = notificationService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
        var targetGroupId = HttpContext.User.IsInRole("Admin") ? 1 : 2;
        var notifications = await _notificationService.GetNotificationsAsync(userId, targetGroupId);

        var model = new UserMenuViewModel
        {
            FullName = $"{user?.FirstName} {user?.LastName}",
            AvatarPath = string.IsNullOrWhiteSpace(user?.AvatarUrl)
                ? "/images/default-avatar.png"
                : "/" + user.AvatarUrl.TrimStart('/'),
            Notifications = notifications.OrderByDescending(n => n.CreatedAt).ToList()
        };

        return View(model);
    }
}
