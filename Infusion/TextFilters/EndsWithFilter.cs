using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.TextFilters
{
    public class EndsWithFilter : ITextFilter
    {
        public string ProhibitedText { get; }

        public EndsWithFilter(string prohibitedText)
        {
            this.ProhibitedText = prohibitedText;
        }

        public bool IsPassing(string text) => !text.EndsWith(ProhibitedText);
    }
}
