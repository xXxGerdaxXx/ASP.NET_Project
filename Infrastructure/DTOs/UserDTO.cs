using System.ComponentModel.DataAnnotations;
using Infrastructure.Enums;

namespace Infrastructure.DTOs
{
    public class UserDTO
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Full name must be at most 100 characters.")]
        public string FullName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        public string Password { get; set; } = null!;

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = null!;

        [Required]
        public bool AcceptTerms { get; set; } 
    }
}
