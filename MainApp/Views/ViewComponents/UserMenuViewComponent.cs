using Infrastructure.Entities;
using Infrastructure.Interfaces;
using MainApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MainApp.Views.ViewComponents;
/// <summary>
/// This view component loads the user menu with the user's name, avatar, and their latest notifications.
///  It gets the current user and their ID, checks if they are an Admin or not to decide which group of notifications to show,
///  and then builds a view model with all the needed data.
///  The view model is passed to a Razor View that renders the user dropdown.
 
///  I used ChatGPT to help me write and structure this component more clearly.
/// </summary>

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
