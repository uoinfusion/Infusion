using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Infusion.Desktop
{
    internal sealed class Options : INotifyPropertyChanged
    {
        private bool alertToastNotificationEnabled;
        private bool conversationToastNotificationEnabled;
        private bool hideWhenMinimized;
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

        public static Options Instance { get; } = new Options();
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}