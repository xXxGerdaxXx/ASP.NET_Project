namespace Infrastructure.Models;

public class RoleEntity
{
    public int RoleId { get; set; }  // Primary Key
    public string RoleName { get; set; } = null!;  // e.g., "Admin", "User", "Manager"

    // Relationship: One Role can be assigned to many Users
    public List<UserEntity>? Users { get; set; } = new List<UserEntity>();
}
