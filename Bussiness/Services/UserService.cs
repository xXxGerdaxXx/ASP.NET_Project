using Infrastructure.DTOs;
using Infrastructure.Models;
using Infrastructure.Repositories;
using Infrastructure.Helpers;
using Infrastructure.Interfaces;

namespace Business.Services;

public class UserService(IUserRepository userRepository)
{
    private readonly IUserRepository _userRepository = userRepository;

    // ✅ Register User
    public async Task<ServiceResponse<string>> RegisterUser(UserDTO userDTO)
    {
        var response = new ServiceResponse<string>();

        // Check if email already exists
        var existingUser = await _userRepository.GetUserByEmailAsync(userDTO.Email);
        if (existingUser != null)
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

        // Hash password
        string passwordHash = PasswordHasher.HashPassword(userDTO.Password);

        // ✅ Generate Username (First word of FullName + Random Number)
        var nameParts = userDTO.FullName.Trim().Split(' ');
        string firstPart = nameParts[0].ToLower();
        string uniqueIdentifier = new Random().Next(1000, 9999).ToString(); // Random 4-digit number
        string generatedUsername = $"{firstPart}{uniqueIdentifier}";

        var user = new UserEntity
        {
            FullName = userDTO.FullName, // ✅ Assign FullName directly
            Username = generatedUsername, // ✅ Use new username logic
            Email = userDTO.Email,
            PasswordHash = passwordHash,
            RoleId = 2 // Default User Role
        };

        var success = await _userRepository.CreateUserAsync(user);
        response.Success = success;
        response.Message = success ? "User registered successfully!" : "User registration failed.";

        return response;
    }

    // ✅ Get All Users
    public async Task<List<UserEntity>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllUsersAsync();
    }

    // ✅ Authenticate User
    public async Task<UserEntity?> AuthenticateUserAsync(string email, string password)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);

        if (user == null || !PasswordHasher.VerifyPassword(password, user.PasswordHash))
        {
            return null; // ❌ Authentication failed
        }

        return user; // ✅ Authentication successful
    }
}
