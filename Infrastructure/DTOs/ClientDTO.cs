using System.ComponentModel.DataAnnotations;

namespace Infrastructure.DTOs;

public class ClientDto
{
    public int Id { get; set; } // Primary Key

    [Required]
    [MaxLength(100)]
    public string ClientName { get; set; } = null!;

    [MaxLength(200)]
    public string ContactPerson { get; set; } = null!;

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; } = null!;

    [MaxLength(15)]
    [Phone]
    public string PhoneNumber { get; set; } = null!;

    [MaxLength(300)]
    public string Address { get; set; } = null!;
}
