using System.ComponentModel.DataAnnotations;

namespace MainApp.Models;

public class ClientCreateFormModel
{
    [Display(Name = "Client Image", Prompt = "Select an image")]
    [DataType(DataType.Upload)]
    public IFormFile? File { get; set; }

    [Display(Name = "Client Name", Prompt = "Enter client name")]
    [DataType(DataType.Text)]
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

    [Display(Name = "Address", Prompt = "Enter address")]
    [DataType(DataType.Text)]
    public string? Address { get; set; }


    public int Id { get; internal set; }
}
