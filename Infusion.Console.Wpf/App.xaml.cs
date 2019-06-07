using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Avalonia;
using Avalonia.Logging.Serilog;

namespace Infusion.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private global::Avalonia.Application avaloniaApplication;
        private CancellationTokenSource applicationClosedTokenSource = new CancellationTokenSource();

        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Length == 2)
            {
                if (e.Args[0] == "command")
                {
                    InterProcessCommunication.SendCommand(e.Args[1]);
                    Shutdown(0);
                }
            }

            CommandLine.Handler.Handle(e.Args);

            Task.Run(() =>
            {
                Thread.CurrentThread.Name = "Avalonia";
                avaloniaApplication = AppBuilder.Configure<AvaloniaApp>()
                    .UsePlatformDetect()
                    .UseReactiveUI()
                    .LogToDebug()
                    .SetExitMode(ExitMode.OnExplicitExit)
                    .AfterSetup(AvaloniaAppInitialized)
                    .SetupWithoutStarting()
                    .Instance;

                avaloniaApplication.Run(applicationClosedTokenSource.Token);
            });
            WaitForAvaloniaApp();

            base.OnStartup(e);
        }

        private ManualResetEvent avaloniaAppInitializedEvent = new ManualResetEvent(false);
        private void AvaloniaAppInitialized(AppBuilder avaloniaAppBuilder) 
            => avaloniaAppInitializedEvent.Set();

        public void WaitForAvaloniaApp() 
            => avaloniaAppInitializedEvent.WaitOne();

        protected override void OnExit(ExitEventArgs e)
        {
            applicationClosedTokenSource.Cancel();

            base.OnExit(e);
        }
    }
}
