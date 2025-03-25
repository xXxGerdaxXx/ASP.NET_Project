using Microsoft.EntityFrameworkCore;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<UserEntity>(options)
{
    public DbSet<StatusEntity> Statuses { get; set; }
    public DbSet<ProjectEntity> Projects { get; set; }
    public DbSet<MemberEntity> Members { get; set; }
    public DbSet<ClientEntity> Clients { get; set; } = default!;
    public DbSet<UserNotificationEntity> Notifications { get; set; }
    public DbSet<FileEntity> Files { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // ✅ Critical for Identity

        modelBuilder.Entity<UserEntity>()
            .HasKey(u => u.Id);

        modelBuilder.Entity<UserEntity>()
            .HasIndex(u => u.Email)
            .IsUnique();
        // ✅ Define Primary Key for User
        modelBuilder.Entity<UserEntity>()
            .HasKey(u => u.Id);

        modelBuilder.Entity<UserEntity>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // ✅ One-to-Many: One User → Many Projects
        modelBuilder.Entity<ProjectEntity>()
            .HasOne(p => p.CreatedByUser)
            .WithMany(u => u.CreatedProjects)
            .HasForeignKey(p => p.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // ✅ Define Primary Keys
        modelBuilder.Entity<StatusEntity>().HasKey(s => s.Id);
        modelBuilder.Entity<ProjectEntity>().HasKey(p => p.Id);
        modelBuilder.Entity<MemberEntity>().HasKey(m => m.Id);
        modelBuilder.Entity<ClientEntity>().HasKey(c => c.Id); 

        // ✅ One-to-Many: One Status → Many Projects
        modelBuilder.Entity<ProjectEntity>()
            .HasOne(p => p.Status)
            .WithMany(s => s.Projects)
            .HasForeignKey(p => p.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ProjectEntity>()
            .Property(p => p.Budget)
            .HasPrecision(18, 2); // ✅ Precision: 18 digits, 2 decimal places


        // ✅ One-to-Many: One Client → Many Projects
        modelBuilder.Entity<ProjectEntity>()
            .HasOne(p => p.Client)
            .WithMany(c => c.Projects)
            .HasForeignKey(p => p.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        // ✅ Many-to-Many: Projects ↔ Members (via ProjectMemberEntity)
        modelBuilder.Entity<ProjectMemberEntity>()
            .HasKey(pm => new { pm.ProjectId, pm.MemberId });

        modelBuilder.Entity<ProjectMemberEntity>()
            .HasOne(pm => pm.Project)
            .WithMany(p => p.ProjectMembers)
            .HasForeignKey(pm => pm.ProjectId);

        modelBuilder.Entity<ProjectMemberEntity>()
            .HasOne(pm => pm.Member)
            .WithMany(m => m.ProjectMembers)
            .HasForeignKey(pm => pm.MemberId);

        // ✅ Many-to-Many: Users ↔ Notifications (via UserNotificationEntity)
        modelBuilder.Entity<UserNotificationEntity>()
            .HasKey(un => new { un.UserId, un.NotificationId });

        modelBuilder.Entity<UserNotificationEntity>()
            .HasOne(un => un.User)
            .WithMany(u => u.UserNotifications)
            .HasForeignKey(un => un.UserId);

        modelBuilder.Entity<UserNotificationEntity>()
            .HasOne(un => un.Notification)
            .WithMany(n => n.UserNotifications)
            .HasForeignKey(un => un.NotificationId);

        modelBuilder.Entity<NotificationEntity>()
            .HasKey(n => n.NotificationId); // ✅ Define primary key for NotificationEntity


        // ✅ One-to-Many: One User → Many Files
        modelBuilder.Entity<FileEntity>()
            .HasOne(f => f.User)
            .WithMany(u => u.Files)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Delete files if the user is deleted

        modelBuilder.Entity<FileEntity>()
            .Property(f => f.FileName)
            .IsRequired()
            .HasMaxLength(255);

        modelBuilder.Entity<FileEntity>()
            .Property(f => f.FilePath)
            .IsRequired()
            .HasMaxLength(255);

        modelBuilder.Entity<FileEntity>()
            .HasKey(f => f.FileId); // ✅ Define primary key for FileEntity



        // ✅ One-to-Many: One Project → Many Files
        modelBuilder.Entity<FileEntity>()
            .HasOne(f => f.Project)
            .WithMany(p => p.Files)
            .HasForeignKey(f => f.ProjectId)
            .OnDelete(DeleteBehavior.Cascade); // Delete files if the project is deleted

    }
}
