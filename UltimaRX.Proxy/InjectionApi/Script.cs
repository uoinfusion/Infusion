using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infusion.Proxy.InjectionApi
{
    public class Script
    {
        private static Script currentScript;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly Action scriptAction;

        private Task scriptTask;

        public Script(Action scriptAction)
        {
            this.scriptAction = scriptAction;
        }

        public int ThreadId { get; private set; }

        public static void Run(Action action) => new Script(action).Run();

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

                RunAction(scriptAction);
                return this;
            }

            currentScript = this;

            scriptTask = Task.Run(() =>
            {
                ThreadId = Thread.CurrentThread.ManagedThreadId;
                Injection.CancellationToken = cancellationTokenSource.Token;
                RunAction(scriptAction);
            });

            return this;
        }

        private void RunAction(Action action)
        {
            try
            {
                Program.Print("Starting script");

                action();
            }
            catch (OperationCanceledException)
            {
                Program.Print("Script cancelled.");
            }
            catch (Exception ex)
            {
                Program.Print(ex.ToString());
                throw;
            }
            finally
            {
                Program.Print("Script finished.");
                Injection.CancellationToken = null;
                currentScript = null;
            }
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