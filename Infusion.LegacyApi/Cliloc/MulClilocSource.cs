using System;
using Ultima;

namespace Infusion.LegacyApi.Cliloc
{
    internal sealed class MulClilocSource : IClilocSource
    {
        private static readonly Lazy<StringList> clilocDictionary = new Lazy<StringList>(() => new StringList("ENU"));

        public string GetString(int id) 
            => clilocDictionary.Value.GetString(id) ?? $"Unknown Cliloc #{id}";
    }
}
