using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities;

public class ClientEntity
{
    public int Id { get; set; } // Primary Key

    [Required]
    [MaxLength(100)]
    public string ClientName { get; set; } = null!;

    [MaxLength(200)]
    public string ContactPerson { get; set; } = null!;

    [MaxLength(100)]
    public string Email { get; set; } = null!;

    [MaxLength(15)]
    public string PhoneNumber { get; set; } = null!;

    [MaxLength(300)]
    public string Address { get; set; } = null!;

    // One Client can have multiple Projects
    public List<ProjectEntity> Projects { get; set; } = new();
}
