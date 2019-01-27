using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Console
{
    public class NullConsole : IConsole
    {
        public void Critical(string message)
        {
        }

        public void Debug(string message)
        {
        }

        public void Error(string message)
        {
        }

        public void Important(string message)
        {
        }

        public void Info(string message)
        {
        }

        public void WriteSpeech(string name, string message, ObjectId? speakerId, Color color, ModelId bodyType, SpeechType type)
        {
        }

        public void WriteLine(ConsoleLineType type, string message)
        {
        }
    }
}
