using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entities;

public class UserNotificationEntity
{
    public int UserId { get; set; }
    public UserEntity User { get; set; } = null!;

    public int NotificationId { get; set; }
    public NotificationEntity Notification { get; set; } = null!;

    public bool IsRead { get; set; } = false; // Track if user has read the notification
}
