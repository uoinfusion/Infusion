using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Parsers.Processors
{
    public sealed class LumberjackingHarvestProcessor : IOutputLogProcessor
    {
        private Dictionary<string, int> Counts { get; } = new Dictionary<string, int>()
        {
            { "spruce", 0 },
            { "chestnut", 0 },
            { "oak", 0 },
            { "teak", 0 }
        };

        public int Total => Counts.Values.Sum();

        public string Result
            => Counts.Select(x => $"{x.Key}: {x.Value}").Aggregate((l, r) => l + ", " + r);

        public void Process(DateTimeOffset time, string line)
        {
            foreach (var wood in Counts.Keys.ToImmutableArray())
            {
                switch (wood)
                {
                    case "spruce":
                        if (line.Contains("System: You put the spruce log"))
                            Counts["spruce"]++;
                        break;
                    case "chestnut":
                        if (line.Contains("System: You put the chestnut log"))
                            Counts["chestnut"]++;
                        break;
                    case "oak":
                        if (line.Contains("System: You put the oak log"))
                            Counts["oak"]++;
                        break;
                    case "teak":
                        if (line.Contains("System: You put the teak log"))
                            Counts["teak"]++;
                        break;
                }
            }
        }
    }
}
