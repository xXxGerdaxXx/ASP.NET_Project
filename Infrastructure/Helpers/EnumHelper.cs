using System;
using System.ComponentModel;
using System.Reflection;

namespace Infrastructure.Helpers;

public static class EnumHelper
{
    public static string GetEnumDescription(Enum value)
    {
        FieldInfo field = value.GetType().GetField(value.ToString());
        DescriptionAttribute? attribute = field?.GetCustomAttribute<DescriptionAttribute>();

        return attribute?.Description ?? value.ToString(); 
    }
}
