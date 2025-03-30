using System;
namespace Infrastructure.Helpers;

public static class DateHelper
{
    public static string FormatDeadline(DateTime? endDate)
    {
        if (!endDate.HasValue)
            return "No deadline";

        var daysLeft = (endDate.Value - DateTime.Now).Days;

        return daysLeft switch
        {
            < 0 => $"Overdue by {Math.Abs(daysLeft)} day(s)",
            0 => "Due today",
            1 => "1 day left",
            < 7 => $"{daysLeft} days left",
            < 14 => "1 week left",
            _ => $"{daysLeft / 7} week(s) left"
        };
    }
}
