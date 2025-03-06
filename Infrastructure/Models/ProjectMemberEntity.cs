using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models;

public class ProjectMemberEntity
{
    public int ProjectId { get; set; }
    public ProjectEntity Project { get; set; } = null!;

    public int MemberId { get; set; }
    public MemberEntity Member { get; set; } = null!;
}
