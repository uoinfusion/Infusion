using System;

namespace Infusion.Parsers
{
    public sealed class OutputLogParser
    {
        private readonly string log;
        private readonly IOutputLogProcessor[] processors;

        public OutputLogParser(string log, params IOutputLogProcessor[] processors)
        {
            this.log = log;
            this.processors = processors;
        }

        public void Parse()
        {
            var lines = log.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            foreach (var line in lines)
            {
                ParseLine(line.Trim());
            }
        }

        private void ParseLine(string line)
        {
            var colonIndex = line.IndexOf(": ", StringComparison.Ordinal);
            if (colonIndex < 0)
                return;

            string timeText = line.Substring(0, colonIndex);
            DateTimeOffset time;
            if (!DateTimeOffset.TryParse(timeText, out time))
                return;

            var processableLine = line.Substring(colonIndex + 1, line.Length - colonIndex - 1).Trim();

            foreach (var processor in processors)
                processor.Process(time, processableLine);
        }
    }
}
