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
    public DbSet<NotificationEntity> Notifications { get; set; }
    public DbSet<NotificationDismissedEntity> DismissedNotifications { get; set; }
    public DbSet<NotificationTypeEntity> NotificationTypes { get; set; }
    public DbSet<NotificationTargetGroupEntity> NotificationTargetGroups { get; set; }
    public DbSet<FileEntity> Files { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); 

        modelBuilder.Entity<UserEntity>()
            .HasKey(u => u.Id);

        modelBuilder.Entity<UserEntity>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<UserEntity>()
            .HasKey(u => u.Id);

        modelBuilder.Entity<UserEntity>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<ProjectEntity>()
            .HasOne(p => p.CreatedByUser)
            .WithMany(u => u.CreatedProjects)
            .HasForeignKey(p => p.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Primary Keys
        modelBuilder.Entity<StatusEntity>().HasKey(s => s.Id);
        modelBuilder.Entity<ProjectEntity>().HasKey(p => p.Id);
        modelBuilder.Entity<MemberEntity>().HasKey(m => m.Id);
        modelBuilder.Entity<ClientEntity>().HasKey(c => c.Id); 

        // One-to-Many: One Status → Many Projects
        modelBuilder.Entity<ProjectEntity>()
            .HasOne(p => p.Status)
            .WithMany(s => s.Projects)
            .HasForeignKey(p => p.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ProjectEntity>()
            .Property(p => p.Budget)
            .HasPrecision(18, 2);  


        // One-to-Many: One Client → Many Projects
        modelBuilder.Entity<ProjectEntity>()
            .HasOne(p => p.Client)
            .WithMany(c => c.Projects)
            .HasForeignKey(p => p.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        // Many-to-Many: Projects ↔ Members (via ProjectMemberEntity)
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

        // One-to-Many: Notification ↔ NotificationDismissed
        modelBuilder.Entity<NotificationDismissedEntity>()
            .HasKey(d => d.Id);

        modelBuilder.Entity<NotificationDismissedEntity>()
            .HasOne(d => d.Notification)
            .WithMany(n => n.DismissedNotifications)
            .HasForeignKey(d => d.NotificationId)
            .OnDelete(DeleteBehavior.Cascade); 

        modelBuilder.Entity<NotificationDismissedEntity>()
            .HasOne(d => d.User)
            .WithMany(u => u.DismissedNotifications)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<NotificationEntity>()
            .HasKey(n => n.Id);

        modelBuilder.Entity<NotificationEntity>()
            .HasOne(n => n.TargetGroup)
            .WithMany(g => g.Notifications)
            .HasForeignKey(n => n.NotificationTargetGroupId);

        modelBuilder.Entity<NotificationTargetGroupEntity>().HasData(
            new NotificationTargetGroupEntity { Id = 1, Name = "Admin" },
            new NotificationTargetGroupEntity { Id = 2, Name = "Users" }
        );

        modelBuilder.Entity<NotificationTypeEntity>().HasData(
            new NotificationTypeEntity { Id = 1, Name = "UserLogin" },
            new NotificationTypeEntity { Id = 2, Name = "UserSignup" },   
            new NotificationTypeEntity { Id = 3, Name = "ProjectAdded" }  
        );

        modelBuilder.Entity<FileEntity>()
            .HasOne(f => f.User)
            .WithMany(u => u.Files)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade); 

        modelBuilder.Entity<FileEntity>()
            .Property(f => f.FileName)
            .IsRequired()
            .HasMaxLength(255);

        modelBuilder.Entity<FileEntity>()
            .Property(f => f.FilePath)
            .IsRequired()
            .HasMaxLength(255);

        modelBuilder.Entity<FileEntity>()
            .HasKey(f => f.FileId); 


        modelBuilder.Entity<FileEntity>()
            .HasOne(f => f.Project)
            .WithMany(p => p.Files)
            .HasForeignKey(f => f.ProjectId)
            .OnDelete(DeleteBehavior.Cascade); 

        modelBuilder.Entity<StatusEntity>().HasData(
            new StatusEntity { Id = 1, StatusName = "Not Started" },
            new StatusEntity { Id = 2, StatusName = "In Progress" },
            new StatusEntity { Id = 3, StatusName = "Completed" }
        );

    }
}
