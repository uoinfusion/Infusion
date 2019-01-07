using Infusion.LegacyApi.Console;
using System;
using System.Windows.Media;
using System.Windows.Threading;

namespace Infusion.Desktop.Console
{
    internal class WpfConsole
    {
        private readonly ConsoleContent consoleContent;
        private readonly Dispatcher dispatcher;

        public WpfConsole(ConsoleContent consoleContent, Dispatcher dispatcher)
        {
            this.consoleContent = consoleContent;
            this.dispatcher = dispatcher;
        }

        internal void WriteSpeech(DateTime timeStamp, string name, string message, string text, Color color)
        {
            DispatchWrtieSpeech(timeStamp, name, message, text, color);
        }

        internal void WriteLine(DateTime timeStamp, ConsoleLineType type, string message)
        {
            switch (type)
            {
                case ConsoleLineType.Debug:
                    DispatchWriteLine(timeStamp, message, Brushes.DimGray);
                    break;
                case ConsoleLineType.Error:
                    DispatchWriteLine(timeStamp, message, Brushes.DarkRed);
                    break;
                case ConsoleLineType.Important:
                    DispatchWriteLine(timeStamp, message, Brushes.White);
                    break;
                case ConsoleLineType.Critical:
                    DispatchWriteLine(timeStamp, message, Brushes.Red);
                    break;
                case ConsoleLineType.Information:
                    DispatchWriteLine(timeStamp, message, Brushes.Gray);
                    break;
                case ConsoleLineType.ScriptEcho:
                    DispatchWriteLine(timeStamp, message, Brushes.LightGray);
                    break;
                case ConsoleLineType.ScriptResult:
                    DispatchWriteLine(timeStamp, message, Brushes.LightSkyBlue);
                    break;
                case ConsoleLineType.Warning:
                    DispatchWriteLine(timeStamp, message, Brushes.Yellow);
                    break;
                case ConsoleLineType.SkillChanged:
                    DispatchWriteLine(timeStamp, message, Brushes.Azure);
                    break;
            }
        }

        private void DispatchWriteLine(DateTime timeStamp, string message, Brush textBrush)
        {
            dispatcher.BeginInvoke((Action)(() => { WriteLine(timeStamp, new ConsoleLine(timeStamp, message, textBrush)); }));
        }

        private void DispatchWrtieSpeech(DateTime timeStamp, string name, string message, string text, Color color)
        {
            dispatcher.BeginInvoke((Action)(() =>
            {
                var drawingColor = Ultima.Hues.GetHue(color - 1).GetColor(31);
                var wpfColor = (drawingColor.GetBrightness() < 0.05)
                    ? System.Windows.Media.Color.FromRgb((byte)(255 - drawingColor.R), (byte)(255 - drawingColor.G), (byte)(255 - drawingColor.B))
                    : System.Windows.Media.Color.FromRgb(drawingColor.R, drawingColor.G, drawingColor.B);

                var brush = new SolidColorBrush(wpfColor);

                WriteLine(timeStamp, new ConsoleSpeechLine(timeStamp, name, message, text, brush));
            }));
        }

        private DateTime? lastWriteLineDate;

        private void WriteLine(DateTime timeStamp, ConsoleLine line)
        {
            EnsureDateChangeInfo(timeStamp);

            consoleContent.Add(line);
        }

        private void EnsureDateChangeInfo(DateTime timeStamp)
        {
            if (!lastWriteLineDate.HasValue || lastWriteLineDate.Value != timeStamp.Date)
            {
                consoleContent.Add(new ConsoleLine($"{timeStamp.Date:d}", Brushes.White));
                lastWriteLineDate = timeStamp.Date;
            }
        }
    }
}
