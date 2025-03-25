using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Infrastructure.Helpers;

public static class EnumExtensions
{
    public static IEnumerable<SelectListItem> GetEnumSelectListWithDescription<T>() where T : Enum
    {
        return Enum.GetValues(typeof(T))
            .Cast<T>()
            .Select(e => new SelectListItem
            {
                Value = e.ToString(),
                Text = e.GetType().GetField(e.ToString())
                    ?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? e.ToString()
            });
    }
}

