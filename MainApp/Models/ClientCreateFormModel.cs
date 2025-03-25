using System.ComponentModel.DataAnnotations;

namespace MainApp.Models;

public class ClientCreateFormModel
{
    public int Id { get; set; } 

    [Display(Name = "Client Image", Prompt = "Select an image")]
    [DataType(DataType.Upload)]
    public IFormFile? File { get; set; }
    public string? AvatarUrl { get; set; }
    public string Status { get; set; } = "Active";

    [Display(Name = "Client Name", Prompt = "Enter client name")]
    [Required(ErrorMessage = "Client name is required")]
    [MaxLength(100)]
    public string ClientName { get; set; } = null!;

    [Display(Name = "Contact Person", Prompt = "Enter contact person")]
    [Required(ErrorMessage = "Contact person is required")]
    [MaxLength(200)]
    public string ContactPerson { get; set; } = null!;

    [Display(Name = "Email", Prompt = "Enter email address")]
    [Required(ErrorMessage = "Email is required")]
    [DataType(DataType.EmailAddress)]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format")]
    [MaxLength(100)]
    public string Email { get; set; } = null!;

    [Display(Name = "Phone Number", Prompt = "Enter phone number")]
    [MaxLength(15)]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Address", Prompt = "Enter address")]
    [MaxLength(300)]
    public string? Address { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
