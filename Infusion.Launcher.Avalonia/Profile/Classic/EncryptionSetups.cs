using Avalonia.Data.Converters;
using Infusion.Avalonia.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.Launcher.Avalonia.Profile.Classic
{
    public static class EncryptionSetups
    {
        public static IList<DescribedValue<EncryptionSetup>> Items { get; }
            = EnumUtils.GetItems<EncryptionSetup>();

        public static IValueConverter Converter { get; }
            = new DescribedValueEnumConverter<EncryptionSetup>(Items);

    }
}
