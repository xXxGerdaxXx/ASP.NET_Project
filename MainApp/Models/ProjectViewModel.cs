namespace MainApp.Models;

public class ProjectViewModel
{
    public string Name { get; set; } = null!;
    public string Company { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Deadline { get; set; } = null!;
    public List<TeamMember> TeamMembers { get; set; } = new();
}

public class TeamMember
{
    public string Name { get; set; } = null!;
    public string AvatarUrl { get; set; } = null!;
}
