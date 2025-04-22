using Infrastructure.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainApp.Models;

public class MemberCreateForm
{

    [Display(Name = "Member Image", Prompt = "Select an image")]
    [DataType(DataType.Upload)]
    public IFormFile? File { get; set; }
    public string? AvatarUrl { get; set; }

    [Display(Name = "First Name", Prompt = "Enter member's name")]
    [Required(ErrorMessage = "First name is required")]
    public string FirstName { get; set; } = null!;

    [Display(Name = "Last Name", Prompt = "Enter member's surname")]
    [Required(ErrorMessage = "Last name is required")]
    public string LastName { get; set; } = null!;

    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email", Prompt = "Enter email address")]
    [Required(ErrorMessage = "Email address is required")]
    [RegularExpression(@"^[^\s]+@[^\s]+\.[^\s]+$", ErrorMessage = "Invalid email")]
    public string Email { get; set; } = null!;

    [DataType(DataType.PhoneNumber)]
    [Display(Name = "Phone", Prompt = "Enter phone number")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Job Title", Prompt = "Select job title")]
    [Required(ErrorMessage = "You must select a job title")]
    public JobTitle? JobTitle { get; set; }

    [Display(Name = "Address", Prompt = "Enter member's address")]
    [Required(ErrorMessage = "Address is required")]
    public string Address { get; set; } = null!;

    [NotMapped]
    [Required(ErrorMessage = "Day is required")]
    public int? BirthDay { get; set; }

    [NotMapped]
    [Required(ErrorMessage = "Month is required")]
    public int? BirthMonth { get; set; }

    [NotMapped]
    [Required(ErrorMessage = "Year is required")]
    public int? BirthYear { get; set; }


}
