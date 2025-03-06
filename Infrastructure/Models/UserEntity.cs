using System.ComponentModel.DataAnnotations;
using Infrastructure.Enums;

namespace Infrastructure.Models
{
    public class UserEntity
    {
        public int UserId { get; set; }  // Primary Key (Defined in Fluent API)

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        public JobTitle JobTitle { get; set; } // Enum for job title

        // Foreign Key to RoleEntity (For Role-Based Access)
        public int RoleId { get; set; }
        public RoleEntity Role { get; set; } = null!;

        // Navigation property for projects created by this user
        public List<ProjectEntity>? CreatedProjects { get; set; } = new();

        public List<UserNotificationEntity> UserNotifications { get; set; } = new();

        public List<FileEntity> Files { get; set; } = new();
    }
}
