using System.ComponentModel.DataAnnotations;


namespace MainApp.Models;

public class AdminSignInFormModel
{
    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "You must enter your email address.")]
    public string Email { get; set; } = null!;

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "You must enter a password.")]
    public string Password { get; set; } = null!;

    public bool RememberMe { get; set; }

    public bool ForgotPasswod { get; set; }

}