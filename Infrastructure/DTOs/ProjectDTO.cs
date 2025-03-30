using System;
namespace Infrastructure.DTOs;

public class ProjectDTO
{
    public string ProjectName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? Budget { get; set; }
    public int ClientId { get; set; }
    public int StatusId { get; set; }
    public string? AvatarUrl { get; set; }
    public string CreatedByUserId { get; set; } = null!;
}
