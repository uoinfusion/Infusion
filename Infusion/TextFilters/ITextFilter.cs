using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.TextFilters
{
    public interface ITextFilter
    {
        bool IsPassing(string text);
    }
}
