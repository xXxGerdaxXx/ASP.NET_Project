using Infrastructure.Enums;
using System;
using System.Collections.Generic;

namespace Infrastructure.Entities;

public class MemberEntity
{
    public int Id { get; set; }  

    public string? AvatarUrl { get; set; }  
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Address { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }
    public JobTitle JobTitle { get; set; } 

    // Many-to-Many: Members ↔ Projects (via join table)
    public List<ProjectMemberEntity> ProjectMembers { get; set; } = new();

    // **Computed Property: Age (Not stored in DB)**
    public int Age
    {
        get
        {
            var today = DateTime.Today;
            int age = today.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}
