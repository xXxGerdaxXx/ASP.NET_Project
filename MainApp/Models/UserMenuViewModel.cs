using Infrastructure.Entities;

namespace MainApp.Models;

public class UserMenuViewModel
{
    public string FullName { get; set; } = null!;
    public string AvatarPath { get; set; } = null!;
    public IEnumerable<NotificationEntity> Notifications { get; set; } = Enumerable.Empty<NotificationEntity>();
}
