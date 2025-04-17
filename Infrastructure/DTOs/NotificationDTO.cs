using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs;

public class NotificationDto
{
    public string Message { get; set; } = null!;
    public string? Icon { get; set; }
    public DateTime CreatedAt { get; set; }
}
