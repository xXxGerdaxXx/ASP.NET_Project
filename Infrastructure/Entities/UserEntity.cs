using System.ComponentModel.DataAnnotations;
using Infrastructure.Enums;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Entities;

public class UserEntity : IdentityUser
{
    [Required]
    [MaxLength(100)]
    [ProtectedPersonalData]
    public string FirstName { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    [ProtectedPersonalData]
    public string LastName { get; set; } = null!;

    public string? AvatarUrl { get; set; }

    public JobTitle JobTitle { get; set; }

    public List<ProjectEntity>? CreatedProjects { get; set; } = [];

    public List<NotificationEntity> UserNotifications { get; set; } = [];

    public ICollection<NotificationDismissedEntity> DismissedNotifications { get; set; } = [];

    public List<FileEntity> Files { get; set; } = [];
}
