using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities
{
    public class NotificationTypeEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public ICollection<NotificationEntity> Notifications { get; set; } = [];
    }
}
