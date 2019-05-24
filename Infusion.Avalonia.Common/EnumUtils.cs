using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Infusion.Avalonia.Common;
using Portable.Xaml.Markup;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;

namespace Infusion.Avalonia.Common
{
    public static class EnumUtils
    {
        public static IList<DescribedValue<T>> GetItems<T>()
        {
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(x => new DescribedValue<T> { Description = GetDisplayName(x), Value = x })
                .ToList();
        }

        private static string GetDisplayName(object e)
        {
            var name = e.ToString();
            var type = e.GetType();
            var memInfo = type.GetMember(name);
            var attribute = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false)
                .OfType<DescriptionAttribute>()
                .SingleOrDefault();

            return attribute?.Description ?? name;
        }
    }
}