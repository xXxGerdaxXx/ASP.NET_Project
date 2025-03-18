using System;
using System.ComponentModel.DataAnnotations;
using Infrastructure.Enums; // Assuming JobTitle is an enum

namespace MainApp.Models
{
    public class MemberEditFormModel
    {
        [Required]
        public int Id { get; set; } // ✅ Required for tracking which member is being edited

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Address { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public JobTitle JobTitle { get; set; } // ✅ Enum (assuming `JobTitle` is an enum)

        public string? AvatarUrl { get; set; } // ✅ Profile image URL

        public IFormFile? File { get; set; } // ✅ For updating the profile picture
    }
}
