using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entities;

public class NotificationEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [ForeignKey("TargetGroup")]
    public int NotificationTargetGroupId { get; set; }
    public NotificationTargetGroupEntity TargetGroup { get; set; } = null!;

    public int NotificationTypeId { get; set; }
    public NotificationTypeEntity NotificationType { get; set; } = null!;

    public string Icon { get; set; } = null!;
    public string Message { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<NotificationDismissedEntity> DismissedNotifications { get; set; } = new List<NotificationDismissedEntity>();

}
