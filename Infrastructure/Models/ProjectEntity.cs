using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Models;

public class ProjectEntity
{
    public int Id { get; set; }  // Primary Key

    public string ProjectName { get; set; } = null!;
    public string Description { get; set; } = null!;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Budget { get; set; }

    // Foreign Key for User who created the project
    public int CreatedByUserId { get; set; }
    public UserEntity CreatedByUser { get; set; } = null!;

    // Foreign Key to StatusEntity
    public int StatusId { get; set; }
    public StatusEntity Status { get; set; } = null!;

    // Foreign Key for Client
    public int ClientId { get; set; }
    public ClientEntity Client { get; set; } = null!;

    public List<ProjectMemberEntity> ProjectMembers { get; set; } = new();

    public List<FileEntity> Files { get; set; } = new();
}

