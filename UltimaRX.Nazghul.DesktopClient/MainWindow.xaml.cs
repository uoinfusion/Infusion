using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Windows.Data.Xml.Dom;
using Microsoft.AspNet.SignalR.Client;
using UltimaRX.Nazghul.Common;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Windows.UI.Notifications;

namespace UltimaRX.Nazghul.DesktopClient
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly HubConnection hubConnection;
        private readonly IHubProxy nazghulHub;
        private readonly ConsoleContent dc = new ConsoleContent();

        private readonly HashSet<string> ignoredMessages = new HashSet<string>
        {
            "Marden: Hej! Ty tam. Ano tebe myslim, Pipka. Chces lamu zadarmo? Ano?",
            "Brinley: Nice speaking to you Pipka",
            "Brinley: Well it was nice speaking to you Pipka but i must go about my business",
            "Cullin: Anna Del Tir ",
            "Keleman: Anna Del Tir "
        };

        public MainWindow()
        {
            InitializeComponent();
            DataContext = dc;
            Loaded += MainWindow_Loaded;

            hubConnection = new HubConnection("http://localhost:9094/");
            nazghulHub = hubConnection.CreateHubProxy("NazghulHub");
            nazghulHub.On<LogMessage>("SendLog", OnLogReceived);
            nazghulHub.On<IEnumerable<string>>("SendAllLogs", OnAllLogsReceived);

            hubConnection.Start().Wait();

            nazghulHub.Invoke("RequestAllLogs");

            var ni = new NotifyIcon();
            ni.Icon = SystemIcons.Shield;
            ni.Visible = true;
            ni.DoubleClick += (sender, args) =>
            {
                Show();
                WindowState = WindowState.Normal;
            };
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                Hide();

            base.OnStateChanged(e);
        }

        private void OnAllLogsReceived(IEnumerable<string> logs)
        {
            Dispatcher.Invoke(() =>
            {
                dc.ConsoleOutput = new ObservableCollection<string>(logs.ToList());
                Scroller.ScrollToBottom();
            });
        }

        private void OnLogReceived(LogMessage log)
        {
            Dispatcher.Invoke(() =>
            {
                dc.Add(log.Message);
                Scroller.ScrollToBottom();

                if (IsIgnoredMessage(log.Message))
                    return;

                if (log.Type == LogMessageType.Speech || log.Type == LogMessageType.Alert)
                {
                    XmlDocument toastXml;
                    if (log.Type == LogMessageType.Speech)
                    {
                        toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);

                        var stringElements = toastXml.GetElementsByTagName("text");
                        stringElements[0].AppendChild(toastXml.CreateTextNode(log.Message));

                        var audioElement = toastXml.CreateElement("audio");
                        audioElement.SetAttribute("silent", "true");
                        toastXml.SelectSingleNode("/toast")?.AppendChild(audioElement);
                    }
                    else
                    {
                        toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

                        var stringElements = toastXml.GetElementsByTagName("text");
                        stringElements[0].AppendChild(toastXml.CreateTextNode("Nazghul Alert"));
                        stringElements[1].AppendChild(toastXml.CreateTextNode(log.Message));
                    }

                    var toast = new ToastNotification(toastXml);
                    toast.ExpirationTime = DateTimeOffset.UtcNow.AddMinutes(10);

                    ToastNotificationManager.CreateToastNotifier("Nazghul Toast").Show(toast);
                }
            });
        }

        private bool IsIgnoredMessage(string message) => ignoredMessages.Contains(message);

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InputBlock.KeyDown += InputBlock_KeyDown;
            InputBlock.Focus();
        }

        private void InputBlock_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RunCommand();
                InputBlock.Focus();
                Scroller.ScrollToBottom();
            }
        }

        public void RunCommand()
        {
            nazghulHub.Invoke<string>("Say", InputBlock.Text);
            dc.ConsoleInput = string.Empty;
        }

        public class ConsoleContent : INotifyPropertyChanged
        {
            private string consoleInput = string.Empty;

            private ObservableCollection<string> consoleOutput = new ObservableCollection<string>
            {
                "Console Emulation Sample..."
            };

            public string ConsoleInput
            {
                get { return consoleInput; }
                set
                {
                    consoleInput = value;
                    OnPropertyChanged("ConsoleInput");
                }
            }

            public ObservableCollection<string> ConsoleOutput
            {
                get { return consoleOutput; }
                set
                {
                    consoleOutput = value;
                    OnPropertyChanged("ConsoleOutput");
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            private void OnPropertyChanged(string propertyName)
            {
                if (null != PropertyChanged)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

            public void Add(string message)
            {
                ConsoleOutput.Add($"{DateTime.Now:hh:mm:ss} - {message}");
                if (ConsoleOutput.Count > 256)
                    ConsoleOutput.RemoveAt(0);
            }
        }
    }
}