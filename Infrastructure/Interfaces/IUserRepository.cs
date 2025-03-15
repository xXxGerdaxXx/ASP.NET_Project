using Infrastructure.Entities;

namespace Infrastructure.Interfaces;

public interface IUserRepository
{
    Task<UserEntity?> GetUserByIdAsync(int id);
    Task<UserEntity?> GetUserByEmailAsync(string email);
    Task<List<UserEntity>> GetAllUsersAsync();
    Task<bool> CreateUserAsync(UserEntity user);
    Task<bool> UpdateUserAsync(UserEntity user);
    Task<bool> DeleteUserAsync(int id);
}



