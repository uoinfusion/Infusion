using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Threading;
using Infusion.Desktop.Profiles;
using Infusion.Logging;
using Infusion.Proxy;

namespace Infusion.Desktop
{
    internal class InfusionConsoleLogger : ITimestampedLogger
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

        public void Info(DateTime timeStamp, string message)
        {
            DispatchWriteLine(timeStamp, message, Brushes.Gray);
        }

        public void Important(DateTime timeStamp, string message)
        {
            DispatchWriteLine(timeStamp, message, Brushes.White);
            ToastNotification(message);
        }

        public void Debug(DateTime timeStamp, string message)
        {
            DispatchWriteLine(timeStamp, message, Brushes.DimGray);
        }

        public void Critical(DateTime timeStamp, string message)
        {
            DispatchWriteLine(timeStamp, message, Brushes.Red);
            ToastAlertNotification(message);
        }

        public void Error(DateTime timeStamp, string message)
        {
            DispatchWriteLine(timeStamp, message, Brushes.DarkRed);
        }

        private void DispatchWriteLine(DateTime timeStamp, string message, Brush textBrush)
        {
            dispatcher.BeginInvoke((Action) (() => { WriteLine(timeStamp, message, textBrush); }));
        }

        private DateTime? lastWriteLineDate;

        private void WriteLine(DateTime timeStamp, string message, Brush textBrush)
        {
            if (!lastWriteLineDate.HasValue || lastWriteLineDate.Value != timeStamp.Date)
            {
                consoleContent.Add($"{timeStamp.Date:d}", Brushes.White);
                lastWriteLineDate = timeStamp.Date;
            }

            consoleContent.Add($"{timeStamp:HH:mm:ss:fff} - {message}", textBrush);
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

            //var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

            //var stringElements = toastXml.GetElementsByTagName("text");
            //stringElements[0].AppendChild(toastXml.CreateTextNode(ProfileRepositiory.SelectedProfile.Name));
            //stringElements[1].AppendChild(toastXml.CreateTextNode(message));

            //var audioElement = toastXml.CreateElement("audio");
            //audioElement.SetAttribute("silent", "true");
            //toastXml.SelectSingleNode("/toast")?.AppendChild(audioElement);

            //var toast = new ToastNotification(toastXml);
            //toast.Group = "Infusion";
            //toast.ExpirationTime = DateTimeOffset.UtcNow.AddMinutes(10);

            //var notifier = ToastNotificationManager.CreateToastNotifier("Infusion");
            //notifier.Show(toast);
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
            //var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

            //var stringElements = toastXml.GetElementsByTagName("text");
            //stringElements[0].AppendChild(toastXml.CreateTextNode(ProfileRepositiory.SelectedProfile.Name));
            //stringElements[1].AppendChild(toastXml.CreateTextNode(message));

            //var toast = new ToastNotification(toastXml);
            //toast.Group = "Infusion";
            //toast.ExpirationTime = DateTimeOffset.UtcNow.AddMinutes(10);

            //var notifier = ToastNotificationManager.CreateToastNotifier("Infusion");
            //notifier.Show(toast);
        }
    }
}