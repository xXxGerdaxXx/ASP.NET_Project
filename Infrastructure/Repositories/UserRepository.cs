using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    private readonly AppDbContext _context = context;

    // ✅ Get All Users
    public async Task<List<UserEntity>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    // ✅ Get User by ID
    public async Task<UserEntity?> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    // ✅ Get User by Email
    public async Task<UserEntity?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    // ✅ Create a New User
    public async Task<bool> CreateUserAsync(UserEntity user)
    {
        _context.Users.Add(user);
        return await _context.SaveChangesAsync() > 0;
    }

    // ✅ Update an Existing User
    public async Task<bool> UpdateUserAsync(UserEntity user)
    {
        _context.Users.Update(user);
        return await _context.SaveChangesAsync() > 0;
    }

    // ✅ Delete User by ID
    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        _context.Users.Remove(user);
        return await _context.SaveChangesAsync() > 0;
    }
}
