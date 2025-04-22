using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Enums; 

namespace MainApp.Models
{
    public class MemberEditFormModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [Display(Name = "First Name")]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [Display(Name = "Last Name")]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [Display(Name = "Phone")]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        [Display(Name = "Address")]
        [StringLength(100)]
        public string Address { get; set; } = string.Empty;

        //[Required]
        //[Display(Name = "Date of Birth")]
        //public DateTime DateOfBirth { get; set; }
        [NotMapped]
        public int BirthDay { get; set; }
        public int BirthMonth { get; set; }
        public int BirthYear { get; set; }

        public DateTime DateOfBirth => new(BirthYear, BirthMonth, BirthDay);

        [Required]
        [Display(Name = "Job Title")]
        public JobTitle JobTitle { get; set; } 

        public string? AvatarUrl { get; set; } 

        public IFormFile? File { get; set; } 
    }
}
