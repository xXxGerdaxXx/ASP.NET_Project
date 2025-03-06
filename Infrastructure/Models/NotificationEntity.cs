using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models;

public class NotificationEntity
{
    public int NotificationId { get; set; } // Primary Key
    public string Message { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Property for Many-to-Many
    public List<UserNotificationEntity> UserNotifications { get; set; } = new();
}
