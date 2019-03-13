using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Infusion.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
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

            base.OnStartup(e);
        }
    }
}
