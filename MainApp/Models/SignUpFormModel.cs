using System.ComponentModel.DataAnnotations;
namespace MainApp.Models;


public class SignUpFormModel
{
    [Required(ErrorMessage = "You must enter your first name.")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "You must enter your last name.")]
    public string LastName { get; set; } = null!;

    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "You must enter your email address.")]
    public string Email { get; set; } = null!;

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "You must enter a password.")]
    public string Password { get; set; } = null!;

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "You must confirm your password.")]
    public string ConfirmPassword { get; set; } = null!;

}
