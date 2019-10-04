using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Desktop.Launcher
{
    internal static class ClientProcessWatcher
    {
        public static void Watch(Process process)
        {
            process.Exited += HandleClientProcessExit;
            process.EnableRaisingEvents = true;

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
                System.Windows.Application.Current.Exit += (sender, e) => {
                    try
                    {
                        process.Kill();
                    }
                    catch
                    {
                        // just ignore failed kill attempt
                    }
                });
        }

        private static void HandleClientProcessExit(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
                System.Windows.Application.Current.Shutdown());
        }
    }
}
