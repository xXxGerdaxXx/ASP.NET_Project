using System;
using System.ComponentModel.DataAnnotations;
using Infrastructure.Enums; 

namespace MainApp.Models
{
    public class MemberEditFormModel
    {
        [Required]
        public int Id { get; set; } 

        [Required]
        [Display(Name = "First Name")]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Phone")]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Address")]
        [StringLength(100)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [Display(Name = "Job Title")]
        public JobTitle JobTitle { get; set; } 

        public string? AvatarUrl { get; set; } 

        public IFormFile? File { get; set; } 
    }
}
