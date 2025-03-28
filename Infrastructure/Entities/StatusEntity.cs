using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entities;

public class StatusEntity
{
    public int Id { get; set; }  // Primary Key
    public string StatusName { get; set; } = null!;

    // Navigation property (One Status → Many Projects)
    public List<ProjectEntity>? Projects { get; set; } = [];
}
