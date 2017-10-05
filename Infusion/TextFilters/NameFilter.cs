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
            try
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

                    if (text[speechIndex] == ' ')
                        speechIndex++;
                }

                for (int i = 0; i < colonIndex; i++)
                {
                    if (speechIndex == text.Length && text[i] == ' ')
                        return false;

                    if (text[i] != text[speechIndex])
                        return true;

                    speechIndex++;
                }

                if (speechIndex < text.Length)
                    return true;

                return false;

            }
            catch (Exception e)
            {
                throw new Exception($"Original text: {text}", e);
            }
        }
    }
}
