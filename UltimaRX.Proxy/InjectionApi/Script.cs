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

        private Task scriptTask;
        private readonly Action scriptAction;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public int ThreadId { get; private set; }

        public static Action Create(Action action) => () =>
        {
            new Script(action).Run();
        };

        public Script Run()
        {
            if (currentScript != null)
            {
                if (currentScript.ThreadId != Thread.CurrentThread.ManagedThreadId)
                    throw new InvalidOperationException("A script already running, terminate it first.");

                scriptAction();
                return this;
            }

            currentScript = this;

            scriptTask = Task.Run(() =>
            {
                ThreadId = Thread.CurrentThread.ManagedThreadId;
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

            return this;
        }

        public Script(Action scriptAction)
        {
            this.scriptAction = scriptAction;
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
