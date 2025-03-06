using Infrastructure.Data;
using Infrastructure.DTOs;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Helpers;

namespace Business.Services;

public class UserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResponse<string>> RegisterUser(UserDTO userDTO)
    {
        var response = new ServiceResponse<string>();

        // Check if email already exists
        if (await _context.Users.AnyAsync(u => u.Email == userDTO.Email))
        {
            response.Success = false;
            response.Message = "Email is already in use.";
            return response;
        }

        // Ensure terms are accepted
        if (!userDTO.AcceptTerms)
        {
            response.Success = false;
            response.Message = "You must accept the Terms and Conditions.";
            return response;
        }

        // Hash password before storing
        string passwordHash = PasswordHasher.HashPassword(userDTO.Password);

        var user = new UserEntity
        {
            FirstName = userDTO.FirstName,
            LastName = userDTO.LastName,
            Username = GenerateUsername(userDTO.FirstName, userDTO.LastName), // Auto-generate
            Email = userDTO.Email,
            PasswordHash = passwordHash,
            RoleId = 2 // Default to "User" role
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        response.Success = true;
        response.Message = "User registered successfully!";
        return response;
    }

    private string GenerateUsername(string firstName, string lastName)
    {
        return $"{firstName.ToLower()}.{lastName.ToLower()}"; // Example: john.doe
    }
}
