using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.TextFilters
{
    public class StartsWithFilter : ITextFilter
    {
        public string ProhibitedText { get; }

        public StartsWithFilter(string prohibitedText)
        {
            this.ProhibitedText = prohibitedText;
        }

        public bool IsPassing(string text) => !text.StartsWith(ProhibitedText);
    }
}
