using Avalonia.Data.Converters;
using Infusion.Avalonia.Common;
using Infusion.Proxy.Launcher;
using System.Collections.Generic;

namespace Infusion.Launcher.Avalonia.Profile
{
    public static class UltimaClientTypes
    {
        public static IList<DescribedValue<UltimaClientType>> Items { get; }
            = EnumUtils.GetItems<UltimaClientType>();

        public static IValueConverter Converter { get; }
            = new DescribedValueEnumConverter<UltimaClientType>(Items);
    }
}
