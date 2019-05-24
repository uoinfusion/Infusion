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

        internal void WriteSpeech(DateTime timeStamp, string name, string message, string text, Color color, ModelId bodyType, SpeechType type)
        {
            dispatcher.BeginInvoke((Action)(() =>
            {
                var drawingColor = Ultima.Hues.GetHue(color - 1).GetColor(31);
                var wpfColor = (drawingColor.GetBrightness() < 0.05)
                    ? System.Windows.Media.Color.FromRgb((byte)(255 - drawingColor.R), (byte)(255 - drawingColor.G), (byte)(255 - drawingColor.B))
                    : System.Windows.Media.Color.FromRgb(drawingColor.R, drawingColor.G, drawingColor.B);

                var brush = new SolidColorBrush(wpfColor);

                WriteLine(timeStamp, new ConsoleSpeechLine(timeStamp, name, message, text, bodyType, type, brush));
            }));
        }

        internal void WriteLine(DateTime timeStamp, ConsoleLineType type, string message)
        {
            dispatcher.BeginInvoke((Action)(() => { WriteLine(timeStamp, new ConsoleInfusionLine(timeStamp, message, type)); }));
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
                consoleContent.Add(new ConsoleInfusionLine(timeStamp, $"{timeStamp.Date:d}", ConsoleLineType.Debug));
                lastWriteLineDate = timeStamp.Date;
            }
        }
    }
}
