using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entities;

public class StatusEntity
{
    public int Id { get; set; }  
    public string StatusName { get; set; } = null!;

    public List<ProjectEntity>? Projects { get; set; } = [];
}
