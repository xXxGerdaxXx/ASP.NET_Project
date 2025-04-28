using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Infrastructure.Entities;

public class NotificationDismissedEntity
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Notification))]
    public string NotificationId { get; set; } = null!;
    public NotificationEntity Notification { get; set; } = null!;

    [ForeignKey(nameof(User))]
    public string UserId { get; set; } = null!;
    public UserEntity User { get; set; } = null!;

    public DateTime DismissedAt { get; set; } = DateTime.Now;
}
