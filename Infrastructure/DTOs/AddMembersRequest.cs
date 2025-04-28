using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs;

public class AddMembersRequest
{
    public int ProjectId { get; set; }
    public List<int> SelectedTeamMemberIds { get; set; } = [];
}
