namespace MainApp.Models;

public class ProjectViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    public string Company { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Deadline { get; set; } = null!;
    public DateTime EndDate { get; set; } 
    public string Status { get; set; } = null!;
    public string? AvatarUrl { get; set; } 
    public List<TeamMember> TeamMembers { get; set; } = new();
}

public class TeamMember
{
    public string Name { get; set; } = null!;
    public string AvatarUrl { get; set; } = null!;
}
