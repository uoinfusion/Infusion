using System;
using System.Windows.Media;

namespace Infusion.Desktop.Console
{
    public class ConsoleLine
    {
        protected static string FormatMessage(DateTime timeStamp, string text) =>
            $"{timeStamp:HH:mm:ss:fff} - {text}";

        public ConsoleLine(DateTime timeStamp, string text, Brush brush)
            : this(FormatMessage(timeStamp, text), brush)
        {
        }

        public ConsoleLine(string text, Brush brush)
        {
            Text = text;
            TextBrush = brush;
        }

        public string Text { get; }

        public Brush TextBrush { get; }
    }

    public class ConsoleSpeechLine : ConsoleLine
    {
        public ConsoleSpeechLine(DateTime timeStamp, string name, string message, string text, Brush brush)
            : base(FormatMessage(timeStamp, text), brush)
        {
            Name = name;
            Message = message;
        }

        public string Name { get; }
        public string Message { get; }
    }
}