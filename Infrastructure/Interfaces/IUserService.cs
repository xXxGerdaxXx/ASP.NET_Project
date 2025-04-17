using Infrastructure.DTOs;
using Infrastructure.Entities;
using Infrastructure.Helpers;

namespace Infrastructure.Interfaces;

public interface IUserService
{
    Task<ServiceResponse<string>> SignUp(UserSignUpDTO userDTO);
    Task<List<UserEntity>> GetAllUsersAsync();
    Task<UserEntity?> AuthenticateUserAsync(string email, string password);
}
