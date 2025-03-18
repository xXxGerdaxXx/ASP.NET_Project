using System;

namespace Infrastructure.DTOs;

public class MemberDTO
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string JobTitle { get; set; } = string.Empty; // Assuming JobTitle is stored as a string in DTO
    public string? AvatarUrl { get; set; } // Optional profile image URL
}
