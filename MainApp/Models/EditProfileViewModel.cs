using System.ComponentModel.DataAnnotations;

namespace MainApp.Models;

public class EditProfileViewModel
{
    [Display(Name = "Update your first name")]
    [Required]
    public string FirstName { get; set; }

    [Display(Name = "Update your surname")]
    [Required]
    public string LastName { get; set; }

    public string? CurrentAvatarUrl { get; set; }

    [Display(Name = "Upload new avatar")]
    public IFormFile? Avatar { get; set; }
}
