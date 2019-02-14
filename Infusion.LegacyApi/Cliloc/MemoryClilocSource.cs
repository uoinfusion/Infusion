using System;
using System.Collections.Generic;

namespace Infusion.LegacyApi.Cliloc
{
    internal sealed class MemoryClilocSource : IClilocSource
    {
        private readonly Dictionary<int, string> dictionary = new Dictionary<int, string>();

        public string GetString(int id) => dictionary[id];
        internal void Add(int id, string text) => dictionary.Add(id, text);
    }
}
