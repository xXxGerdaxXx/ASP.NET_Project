using System.ComponentModel.DataAnnotations;

namespace MainApp.Models;

public class ClientCreateFormModel
{
    [Display(Name = "Client Name", Prompt = "Enter client name")]
    [Required(ErrorMessage = "Required")]

    public string ClientName { get; set; } = null!;

    [Display(Name = "Contact Person", Prompt = "Enter contact person")]
    [Required(ErrorMessage = "Required")]

    public string ContactPerson { get; set; } = null!;

    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email", Prompt = "Enter email address")]
    [Required(ErrorMessage = "Required")]
    [RegularExpression(@"^[^\s]+@[^\s]+\.[^\s]+$", ErrorMessage = "Invalid email")]

    public string Email { get; set; } = null!;

    [DataType(DataType.PhoneNumber)]
    [Display(Name = "Phone", Prompt = "Enter phone number")]

    public string? Phone { get; set; }
}
