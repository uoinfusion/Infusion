﻿using Infusion.LegacyApi;
using Infusion.LegacyApi.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace Infusion.Desktop.Console
{
    internal class WpfConsole
    {
        private readonly ConsoleContent consoleContent;
        private readonly Dispatcher dispatcher;
        private readonly Configuration configuration;

        private readonly Version toastingMinimalOsVersion = new Version(6, 2);

        public WpfConsole(ConsoleContent consoleContent, Dispatcher dispatcher, Configuration configuration)
        {
            this.consoleContent = consoleContent;
            this.dispatcher = dispatcher;
            this.configuration = configuration;
        }

        internal void WriteJournalEntry(DateTime timeStamp, string message, Color color)
        {
            DispatchWriteLine(timeStamp, message, color);
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
                    ToastNotification(message);
                    break;
                case ConsoleLineType.Critical:
                    DispatchWriteLine(timeStamp, message, Brushes.Red);
                    ToastAlertNotification(message);
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
            dispatcher.BeginInvoke((Action)(() => { WriteLine(timeStamp, message, textBrush); }));
        }

        private void DispatchWriteLine(DateTime timeStamp, string message, Color color)
        {
            dispatcher.BeginInvoke((Action)(() =>
            {
                var drawingColor = Ultima.Hues.GetHue(color - 1).GetColor(31);
                var brush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(drawingColor.R, drawingColor.G, drawingColor.B));
                WriteLine(timeStamp, message, brush);
            }));
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