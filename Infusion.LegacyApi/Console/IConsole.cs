using Infusion.LegacyApi.Events;
using Infusion.Logging;

namespace Infusion.LegacyApi.Console
{
    public interface IConsole : ILogger
    {
        void WriteLine(ConsoleLineType type, string message);
        void WriteSpeech(string name, string message, ObjectId? speakerId, Color color, ModelId bodyType, SpeechType type);
    }
}
