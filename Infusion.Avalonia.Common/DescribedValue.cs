using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Infusion.Avalonia.Common
{
    public class DescribedValue<T> : IEquatable<DescribedValue<T>>
    {
        public string Description { get; set; }
        public T Value { get; set; }

        public bool Equals(DescribedValue<T> other) => other.Value.Equals(Value);

        public override string ToString() => Description;
    }

    public class DescribedValueEnumConverter<T> : IValueConverter
    {
        private readonly IEnumerable<DescribedValue<T>> values;

        public DescribedValueEnumConverter(IEnumerable<DescribedValue<T>> values)
        {
            this.values = values;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => values.First(x => x.Value.Equals(value));

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var describedValue = (DescribedValue<T>)value;
            return describedValue.Value;
        }
    }

}