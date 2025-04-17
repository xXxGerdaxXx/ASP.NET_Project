using Infrastructure.DTOs;
using Infrastructure.Entities;
using Infrastructure.Repositories;
using Infrastructure.Helpers;
using Infrastructure.Interfaces;

namespace Business.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;


    public async Task<ServiceResponse<string>> SignUp(UserSignUpDTO userDTO)
    {
        var response = new ServiceResponse<string>();

        var existingUser = await _userRepository.GetUserByEmailAsync(userDTO.Email);
        if (existingUser != null)
        {
            response.Success = false;
            response.Message = "Email is already in use.";
            return response;
        }

        if (!userDTO.AcceptTerms)
        {
            response.Success = false;
            response.Message = "You must accept the Terms and Conditions.";
            return response;
        }

        var nameParts = userDTO.FullName.Trim().Split(' ', 2);
        var firstName = nameParts[0];
        var lastName = nameParts.Length > 1 ? nameParts[1] : "";


        var user = new UserEntity
        {
            FirstName = firstName,
            LastName = lastName,
            Email = userDTO.Email,
            UserName = userDTO.Email, 
            PasswordHash = PasswordHasher.HashPassword(userDTO.Password),

        };
        var createdUser = await _userRepository.CreateAsync(user);
        response.Success = createdUser != null;
        response.Message = response.Success ? "User registered successfully!" : "User registration failed.";


        return response;
    }

    public async Task<List<UserEntity>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<UserEntity?> AuthenticateUserAsync(string email, string password)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);

        if (user == null || !PasswordHasher.VerifyPassword(password, user.PasswordHash))
        {
            return null;
        }

        return user; 
    }
}
