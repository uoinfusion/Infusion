using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Threading;
using Windows.UI.Notifications;
using Infusion.Desktop.Profiles;
using Infusion.Proxy;
using Infusion.Proxy.LegacyApi;
using Infusion.Proxy.Logging;

namespace Infusion.Desktop
{
    internal class InfusionConsoleLogger : ILogger
    {
        private readonly ConsoleContent consoleContent;
        private readonly Dispatcher dispatcher;
        private readonly Configuration configuration;

        private readonly Version toastingMinimalOsVersion = new Version(6, 2);

        public InfusionConsoleLogger(ConsoleContent consoleContent, Dispatcher dispatcher, Configuration configuration)
        {
            this.consoleContent = consoleContent;
            this.dispatcher = dispatcher;
            this.configuration = configuration;
        }

        public void Info(string message)
        {
            DispatchWriteLine(message, Brushes.Gray);
        }

        public void Important(string message)
        {
            DispatchWriteLine(message, Brushes.White);
            ToastNotification(message);
        }

        public void Debug(string message)
        {
            DispatchWriteLine(message, Brushes.DimGray);
        }

        public void Critical(string message)
        {
            DispatchWriteLine(message, Brushes.Red);
            ToastAlertNotification(message);
        }

        public void Error(string message)
        {
            DispatchWriteLine(message, Brushes.DarkRed);
        }

        private void DispatchWriteLine(string message, Brush textBrush)
        {
            dispatcher.BeginInvoke((Action) (() => { WriteLine(message, textBrush); }));
        }

        private DateTime? lastWriteLineDate;

        private void WriteLine(string message, Brush textBrush)
        {
            var now = DateTime.Now;

            if (!lastWriteLineDate.HasValue || lastWriteLineDate.Value != now.Date)
            {
                consoleContent.Add($"{now.Date:d}", Brushes.White);
                lastWriteLineDate = now.Date;
            }

            consoleContent.Add($"{now:HH:mm:ss:fff} - {message}", textBrush);
        }

        private void ToastNotification(string message)
        {
            if (!configuration.ShowImportantToastNotification ||
                !configuration.ToastNotificationEnabled)
            {
                return;
            }

            if (Environment.OSVersion.Version < toastingMinimalOsVersion)
                return;

            ToastNotificationCore(message);
        }

        private void ToastNotificationCore(string message)
        {

            var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

            var stringElements = toastXml.GetElementsByTagName("text");
            stringElements[0].AppendChild(toastXml.CreateTextNode(ProfileRepositiory.SelectedProfile.Name));
            stringElements[1].AppendChild(toastXml.CreateTextNode(message));

            var audioElement = toastXml.CreateElement("audio");
            audioElement.SetAttribute("silent", "true");
            toastXml.SelectSingleNode("/toast")?.AppendChild(audioElement);

            var toast = new ToastNotification(toastXml);
            toast.Group = "Infusion";
            toast.ExpirationTime = DateTimeOffset.UtcNow.AddMinutes(10);

            var notifier = ToastNotificationManager.CreateToastNotifier("Infusion");
            notifier.Show(toast);
        }

        private void ToastAlertNotification(string message)
        {
            if (Environment.OSVersion.Version < toastingMinimalOsVersion)
                return;

            if (!configuration.ToastNotificationEnabled)
            {
                return;
            }

            ToastAlertNotificationCore(message);
        }

        private void ToastAlertNotificationCore(string message)
        {
            var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

            var stringElements = toastXml.GetElementsByTagName("text");
            stringElements[0].AppendChild(toastXml.CreateTextNode(ProfileRepositiory.SelectedProfile.Name));
            stringElements[1].AppendChild(toastXml.CreateTextNode(message));

            var toast = new ToastNotification(toastXml);
            toast.Group = "Infusion";
            toast.ExpirationTime = DateTimeOffset.UtcNow.AddMinutes(10);

            var notifier = ToastNotificationManager.CreateToastNotifier("Infusion");
            notifier.Show(toast);
        }
    }
}