using System;

namespace Infusion.Parsers
{
    public interface IOutputLogProcessor
    {
        void Process(DateTimeOffset time, string line);
        string Result { get; }
    }
}