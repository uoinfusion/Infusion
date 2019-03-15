using Infusion.Diagnostic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Infusion.LegacyApi
{
    public sealed class LogConfiguration : INotifyPropertyChanged, IDiagnosticConfiguration
    {
        private bool logPacketsToFileEnabled;
        private string logPath;
        private readonly object logPathLock = new object();
        private bool logToFileEnabled = true;

        public bool LogToFileEnabled
        {
            get => logToFileEnabled;
            set
            {
                logToFileEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool LogPacketsToFileEnabled
        {
            get => logPacketsToFileEnabled;
            set
            {
                logPacketsToFileEnabled = value;
                OnPropertyChanged();
            }
        }

        public string LogPath
        {
            get => logPath;
            set
            {
                lock (logPathLock)
                {
                    logPath = value;
                }
                OnPropertyChanged();
            }
        }

        public string CurrentLogFile { get; internal set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void SetDefaultLogPath(string logPath)
        {
            if (string.IsNullOrEmpty(logPath))
                return; 

            lock (logPathLock)
            {
                if (string.IsNullOrEmpty(this.logPath))
                {
                    this.logPath = logPath;
                }
            }
        }
    }
}