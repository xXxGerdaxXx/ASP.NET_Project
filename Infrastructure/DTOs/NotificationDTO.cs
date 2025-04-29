using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs;

public class NotificationDto
{
    public int NotificationTypeId { get; set; }
    public string Message { get; set; } = null!;
    public string? Icon { get; set; }
    public int NotificationTargetGroupId { get; set; }
}

