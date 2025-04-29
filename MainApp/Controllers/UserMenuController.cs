//using Infrastructure.Entities;
//using Infrastructure.Interfaces;
//using MainApp.Models;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;

//namespace MainApp.Controllers
//{
//    public class UserMenuController : Controller
//    {
//        private readonly UserManager<UserEntity> _userManager;
//        private readonly INotificationService _notificationService;

//        public UserMenuController(UserManager<UserEntity> userManager, INotificationService notificationService)
//        {
//            _userManager = userManager;
//            _notificationService = notificationService;
//        }

//        [HttpGet]
//        public async Task<IActionResult> Load()
//        {
//            var user = await _userManager.GetUserAsync(User);
//            if (user == null)
//            {
//                return Unauthorized();
//            }

//            var fullName = $"{user.FirstName} {user.LastName}";
//            var avatarPath = string.IsNullOrWhiteSpace(user.AvatarUrl)
//                ? Url.Content("~/images/default-avatar.png")
//                : Url.Content("~/" + user.AvatarUrl.TrimStart('/'));

//            var targetGroupId = User.IsInRole("Admin") ? 1 : 2;
//            var notifications = await _notificationService.GetNotificationsAsync(user.Id, targetGroupId);

//            var model = new UserMenuViewModel
//            {
//                FullName = fullName,
//                AvatarPath = avatarPath,
//                Notifications = notifications
//            };

//            return PartialView("_UserMenu", model);
//        }
//    }
//}
