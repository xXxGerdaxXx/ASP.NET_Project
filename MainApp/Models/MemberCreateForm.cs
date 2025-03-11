using System.ComponentModel.DataAnnotations;

namespace MainApp.Models;

public class MemberCreateForm
{
    [Display(Name = "First Name", Prompt = "Enter members name.")]
    [Required(ErrorMessage = "Required")]

    public string MemberName { get; set; } = null!;

    [Display(Name = "Last Name", Prompt = "Enter members surname.")]
    [Required(ErrorMessage = "Required")]

    public string MemberSurname { get; set; } = null!;
    
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email", Prompt = "Enter email address")]
    [Required(ErrorMessage = "Required")]
    [RegularExpression(@"^[^\s]+@[^\s]+\.[^\s]+$", ErrorMessage = "Invalid email")]

    public string Email { get; set; } = null!;

    [DataType(DataType.PhoneNumber)]
    [Display(Name = "Phone", Prompt = "Enter phone number")]

    public string? Phone { get; set; }

    [Display(Name = "Job Title", Prompt = "Select job title.")]
    [Required(ErrorMessage = "Required")]

    public string JobTitle { get; set; } = null!;



}
