using Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.DTOs;

public class MemberDTO
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date of birth is required")]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Job title is required")]
    [EnumDataType(typeof(JobTitle), ErrorMessage = "Invalid job title")]
    public JobTitle JobTitle { get; set; }

    public string? AvatarUrl { get; set; }
}