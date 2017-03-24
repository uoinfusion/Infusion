using System;

namespace Infusion.Parsers.Processors
{
    public sealed class SkillIncreasedProcessor : IOutputLogProcessor
    {
        private readonly string phrase;

        public SkillIncreasedProcessor(string skillName)
        {
            phrase = "Lumberjacking increased by 0.1 %";
        }

        public int Count { get; private set; }  

        public void Process(DateTimeOffset time, string line)
        {
            if (line.Contains(phrase))
                Count++;
        }

        public string Result => "Lumberjacking gains: " + Count;
    }
}
