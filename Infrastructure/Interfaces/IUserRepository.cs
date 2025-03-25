using Infrastructure.Entities;

namespace Infrastructure.Interfaces;

public interface IUserRepository : IBaseRepository<UserEntity>
{
    Task<UserEntity?> GetUserByEmailAsync(string email);
}
