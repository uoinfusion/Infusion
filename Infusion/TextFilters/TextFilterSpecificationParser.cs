using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.TextFilters
{
    public static class TextFilterSpecificationParser
    {
        public static ITextFilter Parse(string specification)
        {
            if (specification.Equals("{name}", StringComparison.OrdinalIgnoreCase))
                return new NameFilter();

            if (specification.Equals("{number}", StringComparison.OrdinalIgnoreCase))
                return new JustNumberFilter();

            if (specification.StartsWith("*"))
            {
                if (specification.EndsWith("*"))
                    return new ContainsFilter(specification.Substring(1, specification.Length - 2));

                return new EndsWithFilter(specification.Substring(1, specification.Length - 1));
            }

            if (specification.EndsWith("*"))
                return new StartsWithFilter(specification.Substring(0, specification.Length - 1));

            return new ContainsFilter(specification);
        }
    }
}
