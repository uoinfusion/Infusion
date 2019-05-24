using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Infusion.Desktop
{
    public class EnumToItemsSource : MarkupExtension
    {
        private readonly Type _type;

        public EnumToItemsSource(Type type)
        {
            _type = type;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetValues(_type)
                .Cast<object>()
                .Select(e => new { Value = e, DisplayName = GetDisplayName(e) });
        }

        private object GetDisplayName(object e)
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
