using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.TextFilters
{
    public class NameFilter : ITextFilter
    {
        private const string NameSeparator = ": ";

        public bool IsPassing(string text)
        {
            int colonIndex = text.IndexOf(NameSeparator, StringComparison.Ordinal);
            if (colonIndex < 1)
                return true;

            int speechIndex = colonIndex + NameSeparator.Length;
            if (speechIndex >= text.Length)
                return true;

            if (char.IsNumber(text[speechIndex]))
            {
                while (char.IsNumber(text[speechIndex]))
                {
                    speechIndex++;
                    if (speechIndex >= text.Length)
                        return true;
                }

                speechIndex++;
            }

            for (int i = 0; i < colonIndex; i++)
            {
                if (text[i] != text[speechIndex])
                    return true;

                speechIndex++;
            }

            if (speechIndex < text.Length)
                return true;

            return false;
        }
    }
}
