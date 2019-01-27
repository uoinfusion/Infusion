using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Console
{
    public class TextConsole : IConsole
    {
        public void Critical(string message)
        {
            System.Console.WriteLine(message);
        }

        public void Debug(string message)
        {
            System.Console.WriteLine(message);
        }

        public void Error(string message)
        {
            System.Console.WriteLine(message);
        }

        public void Important(string message)
        {
            System.Console.WriteLine(message);
        }

        public void Info(string message)
        {
            System.Console.WriteLine(message);
        }

        public void WriteSpeech(string name, string message, ObjectId? speakerId, Color color, ModelId bodyType, SpeechType type)
        {
            System.Console.WriteLine($"{name ?? string.Empty}: {message}");
        }

        public void WriteLine(ConsoleLineType type, string message)
        {
            System.Console.WriteLine(message);
        }
    }
}
