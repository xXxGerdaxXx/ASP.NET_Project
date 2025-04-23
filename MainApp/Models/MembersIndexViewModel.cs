using Infrastructure.Entities;

namespace MainApp.Models;

public class MembersIndexViewModel
{
    public IEnumerable<MemberEntity> Members { get; set; } = [];

    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalMembers { get; set; }

    public MemberCreateForm MemberCreateForm { get; set; } = new();


    public IEnumerable<int> PageSizeOptions { get; } = [2, 3, 4, 5, 6];
}
