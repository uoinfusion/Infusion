using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.TextFilters
{
    public class JustNumberFilter : ITextFilter
    {
        public bool IsPassing(string text)
        {
            int speechIndex = text.IndexOf(SpeechSeparator, StringComparison.Ordinal);
            if (speechIndex > 0)
            {
                speechIndex += SpeechSeparator.Length;
                while (speechIndex < text.Length && char.IsNumber(text[speechIndex]))
                    speechIndex++;

                if (speechIndex == text.Length)
                    return false;
            }

            return true;
        }

        private static string SpeechSeparator => ": ";
    }
}
