using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Infusion.Desktop.Profiles
{
    internal sealed class Options : INotifyPropertyChanged
    {
        private bool alertToastNotificationEnabled;
        private bool conversationToastNotificationEnabled;
        private bool hideWhenMinimized;
        private bool logToFileEnabled;
        private string logPath;
        public bool CanShowToastNotification => Environment.OSVersion.Version.Major >= 6;

        public bool ConversationToastNotificationEnabled
        {
            get { return conversationToastNotificationEnabled; }
            set
            {
                conversationToastNotificationEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool AlertToastNotificationEnabled
        {
            get { return alertToastNotificationEnabled; }
            set
            {
                alertToastNotificationEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool HideWhenMinimized
        {
            get { return hideWhenMinimized; }
            set
            {
                hideWhenMinimized = value;
                OnPropertyChanged();
            }
        }

        public bool LogToFileEnabled
        {
            get { return logToFileEnabled; }
            set
            {
                logToFileEnabled = value; 
                OnPropertyChanged();
            }
        }

        public string LogPath
        {
            get { return logPath; }
            set
            {
                logPath = value; 
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}