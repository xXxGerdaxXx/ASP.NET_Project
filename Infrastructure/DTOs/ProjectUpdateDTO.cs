using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs;

public class ProjectUpdateDTO
{
    public int Id { get; set; }
    public string ProjectName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<int> TeamMemberIds { get; set; } = new List<int>();
    public decimal? Budget { get; set; }
    public int ClientId { get; set; }
    public int StatusId { get; set; }
    public string? AvatarUrl { get; set; }
    public string CreatedByUserId { get; set; } = null!;
}

