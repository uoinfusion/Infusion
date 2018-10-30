using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;
using RoslynPad;
using RoslynPad.UI;

namespace Infusion.Desktop
{
    internal sealed class ScriptOutput : IScriptOutput
    {
        private readonly Dispatcher dispatcher;
        private readonly ConsoleContent content;

        internal ScriptOutput(Dispatcher dispatcher, ConsoleContent content)
        {
            this.dispatcher = dispatcher;
            this.content = content;
        }

        private void Add(string text, Brush brush)
        {
            dispatcher.Invoke(() => { content.Add(text, brush); });
        }

        public void Echo(string text)
        {
            Add(text, Brushes.LightGray);
        }

        public void Error(string text)
        {
            Add(text, Brushes.DarkRed);
        }

        public void Info(string text)
        {
            Add("// " + text, Brushes.DarkGreen);
        }

        public void Result(string text)
        {
            Add(text, Brushes.LightSkyBlue);
        }

        public void Debug(string text)
        {
            Add(text, Brushes.DimGray);
        }
    }
}
