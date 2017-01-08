using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UltimaRX.Proxy.InjectionApi
{
    public class Script
    {
        private static Script currentScript;

        private readonly Task scriptTask;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public static Script Run(Action scriptAction) => new Script(scriptAction);

        public Script(Action scriptAction)
        {
            if (currentScript != null)
                throw new InvalidOperationException("A script already running, terminate it first.");

            currentScript = this;

            scriptTask = Task.Run(() =>
            {
                Injection.CancellationToken = cancellationTokenSource.Token;
                try
                {
                    Program.Print("Starting script");

                    scriptAction();
                }
                catch (OperationCanceledException)
                {
                    Program.Print("Script cancelled.");
                }
                finally
                {
                    Program.Print("Script finished.");
                    Injection.CancellationToken = null;
                    currentScript = null;
                }
            });
        }

        public static void Terminate()
        {
            currentScript?.Stop();
        }


        public void Stop()
        {
            cancellationTokenSource.Cancel();
            scriptTask.Wait(5000);
            if (scriptTask.IsCompleted || scriptTask.IsCanceled || scriptTask.IsFaulted)
            {
                cancellationTokenSource.Dispose();
                currentScript = null;
            }
            else
            {
                throw new InvalidOperationException("Cannot cancel script.");
            }
        }
    }
}
