using Infusion.Commands;
using System;
using System.Text;
using System.Threading;

namespace Infusion.Tests.Commands
{
    public sealed class TestCommand
    {
        private readonly Action additionalAction;

        private readonly EventWaitHandle additionalActionFinished = new EventWaitHandle(false,
            EventResetMode.ManualReset);

        private readonly EventWaitHandle finishedEvent = new EventWaitHandle(false, EventResetMode.ManualReset);
        private readonly EventWaitHandle finishEvent = new EventWaitHandle(false, EventResetMode.ManualReset);

        private readonly EventWaitHandle initializeEvent = new EventWaitHandle(false, EventResetMode.ManualReset);
        private readonly StringBuilder trace = new StringBuilder(1024);

        public TestCommand(CommandHandler handler, string name, Action additionalAction)
            : this(handler, name, CommandExecutionMode.Normal, additionalAction)
        {
        }

        public TestCommand(CommandHandler handler, string name, CommandExecutionMode executionMode,
            Action additionalAction)
        {
            Command = new Command(name, CommandAction, executionMode: executionMode);

            this.additionalAction = additionalAction;

            if (handler != null)
                handler.RunningCommandRemoved += HandlerOnRunnigCommandRemoved;
        }

        public TestCommand(string name) : this(null, name, CommandExecutionMode.Normal, () => { })
        {
        }

        public Command Command { get; }

        private void CommandOnStopped(object sender, CommandInvocation eventArgs)
        {
            trace.AppendLine("CommandOnStopped: OnEntry");
            finishedEvent.Set();
            trace.AppendLine("CommandOnStopped: OnExit");
        }

        private void HandlerOnRunnigCommandRemoved(object sender, CommandInvocation invocation)
        {
            if (invocation.CommandName.Equals(Command.Name, StringComparison.Ordinal))
            {
                trace.AppendLine("HandlerOnCommandStopped: OnEntry");
                finishedEvent.Set();
                trace.AppendLine("HandlerOnCommandStopped: OnExit");
            }
        }

        public void Finish()
        {
            trace.AppendLine("Finish: OnStart");
            finishEvent.Set();
            trace.AppendLine("Finish: OnExit");
        }

        public void WaitForInitialization()
        {
            trace.AppendLine("WaitForInitialiation: OnEntry");
            initializeEvent.AssertWaitOneSuccess();
            trace.AppendLine("WaitForInitialiation: OnExit");
        }

        public bool WaitForFinished() => WaitForFinished(TimeSpan.FromSeconds(1));

        public bool WaitForFinished(TimeSpan timeout)
        {
            trace.AppendLine("WaitForFinished: OnEntry");
            var result = finishedEvent.WaitOne(timeout);
            trace.AppendLine("WaitForFinished: OnExit");

            return result;
        }

        private void CommandAction()
        {
            trace.AppendLine("CommandAction: OnEntry");

            initializeEvent.Set();

            additionalAction?.Invoke();
            additionalActionFinished.Set();

            finishEvent.WaitOneSlow();

            trace.AppendLine("CommandAction: OnExit");
        }

        public void Reset()
        {
            trace.Clear();
            initializeEvent.Reset();
            finishEvent.Reset();
            finishedEvent.Reset();
            additionalActionFinished.Reset();
        }

        public override string ToString() => trace.ToString();

        public void WaitForAdditionalAction()
        {
            additionalActionFinished.AssertWaitOneSuccess();
        }
    }
}
