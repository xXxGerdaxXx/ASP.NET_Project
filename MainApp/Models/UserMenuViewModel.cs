using Infrastructure.Entities;

namespace MainApp.Models;

public class UserMenuViewModel
{
    public string FullName { get; set; }
    public string AvatarPath { get; set; }
    public IEnumerable<NotificationEntity> Notifications { get; set; }
}
