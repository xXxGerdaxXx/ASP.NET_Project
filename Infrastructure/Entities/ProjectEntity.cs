using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Entities;

public class ProjectEntity
{
    public int Id { get; set; } 

    [Required]
    public string ProjectName { get; set; } = null!;
    public string Description { get; set; } = null!;

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? Budget { get; set; }

    public string CreatedByUserId { get; set; } = null!;
    public UserEntity CreatedByUser { get; set; } = null!;

    public int? StatusId { get; set; }
    public StatusEntity Status { get; set; } = null!;

    public int? ClientId { get; set; }
    public ClientEntity Client { get; set; } = null!;

    public string? AvatarUrl { get; set; }

    public List<ProjectMemberEntity> ProjectMembers { get; set; } = new();

    public List<FileEntity> Files { get; set; } = new();
}

