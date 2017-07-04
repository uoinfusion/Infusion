using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Threading;

namespace Infusion.Desktop
{
    public sealed class ConsoleContent : INotifyPropertyChanged
    {
        private ObservableCollection<ConsoleLine> consoleOutput = new ObservableCollection<ConsoleLine>();

        public ObservableCollection<ConsoleLine> ConsoleOutput
        {
            get
            {
                return consoleOutput;
            }
            set
            {
                consoleOutput = value;
                OnPropertyChanged("ConsoleOutput");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Add(string message, Brush brush)
        {
            if (ConsoleOutput.Count > 1024)
                ConsoleOutput.RemoveAt(0);
            ConsoleOutput.Add(new ConsoleLine(message, brush));
        }

        public void Clear()
        {
            consoleOutput.Clear();
        }
    }

    public struct ConsoleLine
    {
        public ConsoleLine(string message, Brush brush)
        {
            Message = message;
            TextBrush = brush;
        }

        public string Message { get; }

        public Brush TextBrush { get; }
    }
}