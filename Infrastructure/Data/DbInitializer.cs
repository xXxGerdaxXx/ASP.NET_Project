using Infrastructure.Models;
using Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data;

public static class DbInitializer
{
    public static void SeedDatabase(AppDbContext context)
    {
        if (!context.Roles.Any()) // Add roles if they don't exist
        {
            context.Roles.AddRange(
                new RoleEntity { RoleId = 1, RoleName = "Admin" },
                new RoleEntity { RoleId = 2, RoleName = "User" }
            );
            context.SaveChanges();
        }

        if (!context.Users.Any(u => u.RoleId == 1)) // Check if an Admin exists
        {
            var admin = new UserEntity
            {
                FullName = "System Admin", // ✅ Replaced FirstName & LastName with FullName
                Username = "admin",
                Email = "admin@example.com",
                PasswordHash = PasswordHasher.HashPassword("Admin123!"),
                RoleId = 1 // Assign Admin role
            };

            context.Users.Add(admin);
            context.SaveChanges();
        }
    }
}
