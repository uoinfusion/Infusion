using Infusion.LegacyApi.Console;
using System;
using System.Windows.Media;

namespace Infusion.Desktop.Console
{
    public abstract class ConsoleLine
    {
        protected static string FormatMessage(DateTime timeStamp, string text) =>
            $"{timeStamp:HH:mm:ss:fff} - {text}";

        protected ConsoleLine(DateTime timeStamp, string text, Brush brush)
            : this(FormatMessage(timeStamp, text), brush)
        {
        }

        protected ConsoleLine(string text, Brush brush)
        {
            Text = text;
            TextBrush = brush;
        }

        public string Text { get; }

        public Brush TextBrush { get; }
    }

    public class ConsoleInfusionLine : ConsoleLine
    {
        public ConsoleInfusionLine(DateTime timeStamp, string text, ConsoleLineType type)
            : base(timeStamp, text, GetBrush(type))
        {
            Type = type;
        }

        public ConsoleLineType Type { get; }

        private static Brush GetBrush(ConsoleLineType type)
        {
            switch (type)
            {
                case ConsoleLineType.Debug:
                    return Brushes.DimGray;
                case ConsoleLineType.Error:
                    return Brushes.DarkRed;
                case ConsoleLineType.Important:
                    return Brushes.White;
                case ConsoleLineType.Critical:
                    return Brushes.Red;
                case ConsoleLineType.Information:
                    return Brushes.Gray;
                case ConsoleLineType.ScriptEcho:
                    return Brushes.LightGray;
                case ConsoleLineType.ScriptResult:
                    return Brushes.LightSkyBlue;
                case ConsoleLineType.Warning:
                    return Brushes.Yellow;
                case ConsoleLineType.SkillChanged:
                    return Brushes.Azure;
                default:
                    throw new NotImplementedException($"ConsoleLineType {type}");
            }
        }
    }

    public class ConsoleSpeechLine : ConsoleLine
    {
        public ConsoleSpeechLine(DateTime timeStamp, string name, string message, string text, ModelId bodyType, SpeechType type, Brush brush)
            : base(FormatMessage(timeStamp, text), brush)
        {
            Name = name;
            Message = message;
            IsSpeech = !string.IsNullOrEmpty(name) && bodyType != 0 && type == SpeechType.Speech;
        }

        public bool IsSpeech { get; }
        public string Name { get; }
        public string Message { get; }
    }
}