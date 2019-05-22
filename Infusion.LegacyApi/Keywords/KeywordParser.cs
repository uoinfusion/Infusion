using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ultima;

namespace Infusion.LegacyApi.Keywords
{
    public sealed class KeywordParser
    {
        private readonly IKeywordSource keywordSource;

        public KeywordParser(IKeywordSource keywordSource)
        {
            this.keywordSource = keywordSource;
        }

        public ushort[] GetKeywordIds(string text)
        {
            text = text.ToLower();
            var list = new List<SpeechEntry>();

            foreach (var entry in keywordSource.Entries)
            {
                if (IsMatch(text, entry))
                    list.Add(entry);
            }

            list.Sort();

            return list.Select(x => (ushort)x.ID).ToArray();
        }

        public bool IsMatch(string input, SpeechEntry entry)
        {
            string[] split = entry.Keywords;

            for (int i = 0; i < split.Length; i++)
            {
                if (split[i].Length > 0 && split[i].Length <= input.Length)
                {
                    if (!entry.CheckStart)
                    {
                        if (input.IndexOf(split[i], 0) < 0)
                            continue;
                    }

                    if (!entry.CheckEnd)
                    {
                        if (input.IndexOf(split[i], input.Length - split[i].Length) < 0)
                            continue;
                    }

                    if (input.IndexOf(split[i]) >= 0)
                        return true;
                }
            }

            return false;
        }
    }
}
