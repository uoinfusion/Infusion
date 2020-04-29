using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Infusion.Commands;
using Infusion.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// ReSharper disable MethodSupportsCancellation

namespace Infusion.Tests.Commands
{
    [TestClass]
    public class CommandHandlerTests
    {
        private CommandHandler commandHandler;
        private CancellationToken cancellationToken;
        private RingBufferLogger logger;

        private void DoSomeCancellableAction()
        {
            while (!cancellationToken.IsCancellationRequested)
                Thread.Yield();
        }

        [TestInitialize]
        public void Initialize()
        {
            logger = new RingBufferLogger(16);
            commandHandler = new CommandHandler(logger);
            commandHandler.CancellationTokenCreated += (sender, token) => cancellationToken = token;
        }

        [TestMethod]
        public void Can_register_two_commands_with_different_names()
        {
            commandHandler.RegisterCommand("testName1", () => { });
            commandHandler.RegisterCommand("testName2", () => { });
        }

        [TestMethod]
        public void Can_invoke_parameterless_command()
        {
            var ev = new AutoResetEvent(false);
            commandHandler.RegisterCommand("testName", () => ev.Set());

            commandHandler.Invoke("testName");
            ev.AssertWaitOneSuccess();
        }

        [TestMethod]
        public void Can_invoke_parametrized_command()
        {
            var actualParameters = string.Empty;
            var ev = new AutoResetEvent(false);

            commandHandler.RegisterCommand("testName", parameters =>
            {
                actualParameters = parameters;
                ev.Set();
            });

            commandHandler.Invoke("testName", "parameter1 parameter2");
            ev.AssertWaitOneSuccess();
            actualParameters.Should().Be("parameter1 parameter2");
        }

        [TestMethod]
        public void Can_invoke_syntax_with_parameterless_command()
        {
            var ev = new AutoResetEvent(false);
            commandHandler.RegisterCommand("testName", () => ev.Set());

            commandHandler.InvokeSyntax(",testName");
            ev.AssertWaitOneSuccess();
        }
        
        [TestMethod]
        public void Can_invoke_syntax_with_parameterless_command_with_custom_prefix()
        {
            var ev = new AutoResetEvent(false);
            commandHandler.RegisterCommand("testName", () => ev.Set());
            commandHandler.CommandPrefix = "[";

            commandHandler.InvokeSyntax("[testName");
            ev.AssertWaitOneSuccess();
        }

        [TestMethod]
        public void Can_register_parametrized_command()
        {
            commandHandler.RegisterCommand("testName", parameters => { });
        }

        [TestMethod]
        public void Can_invoke_syntax_with_parametrized_command()
        {
            var actualParameters = string.Empty;
            var ev = new AutoResetEvent(false);

            commandHandler.RegisterCommand("testName", parameters =>
            {
                actualParameters = parameters;
                ev.Set();
            });

            commandHandler.InvokeSyntax(",testName parameter1 parameter2");
            ev.AssertWaitOneSuccess();
            actualParameters.Should().Be("parameter1 parameter2");
        }

        [TestMethod]
        public void Does_not_invoke_syntax_with_parameterless_command_with_parameters()
        {
            bool executed = false;
            var command = new TestCommand(commandHandler, ",testName", () => executed = true);
            commandHandler.RegisterCommand(command.Command);

            commandHandler.InvokeSyntax(",testName parameter1");

            executed.Should().BeFalse();
        }

        [TestMethod]
        public void Can_invoke_syntax_with_parametrized_command_without_parameters()
        {
            bool executed = false;
            var finished = new AutoResetEvent(false);
            commandHandler.RegisterCommand("testName", () =>
            {
                executed = true;
                finished.Set();
            });

            commandHandler.InvokeSyntax(",testName");
            finished.AssertWaitOneSuccess();

            executed.Should().BeTrue();
        }

        [TestMethod]
        public void Can_return_registered_command_names()
        {
            commandHandler.RegisterCommand("testName", () => { });
            commandHandler.RegisterCommand("testName2", parameters => { });

            commandHandler.CommandNames.Should().Contain(",testName").And.Contain(",testName2");
        }

        [TestMethod]
        public void Can_recognize_command_invocation_syntax()
        {
            commandHandler.IsInvocationSyntax(",testName").Should().BeTrue();
            commandHandler.IsInvocationSyntax("testName").Should().BeFalse();
        }

        [TestMethod]
        public void Can_unregister_parameterless_command()
        {
            commandHandler.RegisterCommand("testname1", param => { });
            commandHandler.RegisterCommand("testname2", () => { });

            commandHandler.Unregister("testname2");

            commandHandler.CommandNames.Should().BeEquivalentTo(",testname1");
        }

        [TestMethod]
        public void Can_unregister_parametrized_command()
        {
            commandHandler.RegisterCommand("testname1", param => { });
            commandHandler.RegisterCommand("testname2", () => { });

            commandHandler.Unregister("testname1");

            commandHandler.CommandNames.Should().BeEquivalentTo(",testname2");
        }

        [TestMethod]
        public void Can_list_running_commands()
        {
            var command1 = new TestCommand("cmd1");
            var command2 = new TestCommand("cmd2");
            commandHandler.RegisterCommand(command1.Command);
            commandHandler.RegisterCommand(command2.Command);
            commandHandler.InvokeSyntax(",cmd1");
            commandHandler.InvokeSyntax(",cmd2");

            command1.WaitForInitialization();
            command2.WaitForInitialization();

            commandHandler.RunningCommands.Select(c => c.Name).Should().Contain("cmd1", "cmd2");

            command1.Finish();
            command2.Finish();
        }

        [TestMethod]
        public void Can_tell_whether_a_command_is_running()
        {
            var command1 = new TestCommand("cmd1");
            commandHandler.RegisterCommand(command1.Command);
            commandHandler.InvokeSyntax(",cmd1");

            command1.WaitForInitialization();

            commandHandler.IsCommandRunning("cmd1").Should().BeTrue("cmd1 command is running");
            commandHandler.IsCommandRunning("cmd2").Should().BeFalse("cmd2 command is not running");

            command1.Finish();
        }

        [TestMethod]
        public void Can_remove_finished_command_from_list()
        {
            var finishedCommand = new TestCommand(commandHandler, "finished_cmd", () =>{ });
            commandHandler.RegisterCommand(finishedCommand.Command);
            commandHandler.InvokeSyntax(",finished_cmd");
            finishedCommand.Finish();
            finishedCommand.WaitForFinished();

            var runningCommand = new TestCommand("running_cmd");
            commandHandler.RegisterCommand(runningCommand.Command);
            commandHandler.InvokeSyntax(",running_cmd");
            runningCommand.WaitForInitialization();

            commandHandler.RunningCommands.Select(c => c.Name).Should().NotContain("finished_cmd");
            commandHandler.RunningCommands.Select(c => c.Name).Should().Contain("running_cmd");
        }

        [TestMethod]
        public void Can_terminate_specific_running_command_multipletimes()
        {
            int executionCount = 0;
            EventWaitHandle counterDone = new EventWaitHandle(false, EventResetMode.ManualReset);

            var command = new Command("cmd", () =>
            {
                executionCount++;
                counterDone.Set();
                DoSomeCancellableAction();
            });
            commandHandler.RegisterCommand(command);

            for (var i = 0; i < 10; i++)
            {
                int oldExecutionCount = executionCount;
                counterDone.Reset();
                Task.Run(() => commandHandler.Invoke("cmd"));

                counterDone.AssertWaitOneSuccess();
                commandHandler.Terminate("cmd", AssertionExtensions.FastTimeout);

                commandHandler.RunningCommands.Should().BeEmpty();
                executionCount.Should().Be(oldExecutionCount + 1);
            }

            executionCount.Should().Be(10);
        }

        [TestMethod]
        public void Can_terminate_only_specific_running_command()
        {
            var command = new TestCommand(commandHandler, "cmd", () => DoSomeCancellableAction());
            commandHandler.RegisterCommand(command.Command);
            commandHandler.InvokeSyntax(",cmd");
            command.WaitForInitialization();

            commandHandler.Terminate("non_existing_command");
            command.Finish();

            command.WaitForFinished(TimeSpan.FromMilliseconds(50)).Should().BeFalse();
        }

        [TestMethod]
        public void Can_terminate_all_running_commands_multiple_times()
        {
            var command1 = new TestCommand(commandHandler, "cmd1", () => DoSomeCancellableAction());
            commandHandler.RegisterCommand(command1.Command);

            var command2 = new TestCommand(commandHandler, "cmd2", () => DoSomeCancellableAction());
            commandHandler.RegisterCommand(command2.Command);

            for (var i = 0; i < 10; i++)
            {
                command1.Reset();
                command2.Reset();
                commandHandler.InvokeSyntax(",cmd1");
                command1.WaitForInitialization();
                commandHandler.InvokeSyntax(",cmd2");
                command2.WaitForInitialization();

                commandHandler.RunningCommands.Should().NotBeEmpty();

                commandHandler.BeginTerminate();

                command1.Finish();
                command1.WaitForFinished().Should().BeTrue();
                command2.Finish();
                command2.WaitForFinished().Should().BeTrue();

                commandHandler.RunningCommands.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void Can_terminate_parent_command_executing_Normal_nested_command()
        {
            var nestedCommand = new TestCommand(commandHandler, "nested", CommandExecutionMode.Normal, () => DoSomeCancellableAction());
            commandHandler.RegisterCommand(nestedCommand.Command);

            var parentCommand = new TestCommand(commandHandler, "parent", () => commandHandler.InvokeSyntax(",nested"));
            commandHandler.RegisterCommand(parentCommand.Command);

            commandHandler.InvokeSyntax(",parent");
            parentCommand.WaitForInitialization();

            commandHandler.Terminate("parent");

            nestedCommand.Finish();
            parentCommand.Finish();

            parentCommand.WaitForFinished().Should().BeTrue();

            commandHandler.RunningCommands.Should().BeEmpty();
        }

        [TestMethod]
        public void Exception_thrown_from_nested_command_propagates_to_parent_command()
        {
            bool nestedCommandExecuted = false;
            var nestedCommand = new TestCommand(commandHandler, "nested", CommandExecutionMode.Normal, () =>
            {
                nestedCommandExecuted = true;
                throw new InvalidOperationException();
            });
            commandHandler.RegisterCommand(nestedCommand.Command);

            bool exceptionThrownFromNestedCommand = false;
            var parentCommand = new TestCommand(commandHandler, "parent", () =>
            {
                try
                {
                    commandHandler.InvokeSyntax(",nested");
                }
                catch (InvalidOperationException)
                {
                    exceptionThrownFromNestedCommand = true;
                }
            });
            commandHandler.RegisterCommand(parentCommand.Command);

            nestedCommand.Finish();
            parentCommand.Finish();
            commandHandler.InvokeSyntax(",parent");

            parentCommand.WaitForFinished();

            nestedCommandExecuted.Should().BeTrue();
            exceptionThrownFromNestedCommand.Should().BeTrue();
        }

        [TestMethod]
        public void Exception_is_handled_when_parent_command_throws_exception()
        {
            var parentCommand = new TestCommand(commandHandler, "parent", () => throw new InvalidOperationException("some exception message that is supposed to appear in the logger output"));
            commandHandler.RegisterCommand(parentCommand.Command);
            commandHandler.InvokeSyntax(",parent");

            parentCommand.WaitForFinished();

            logger.DumpToString().Should().Contain("some exception message that is supposed to appear in the logger output");
        }

        [TestMethod]
        public void Can_execute_nested_Normal_command_from_Normal_parent_on_same_thread()
        {
            int nestedCommandThreadId = -1;
            int parentCommandThreadId = -1;

            var nestedCommandExecuted = false;
            var nestedCommand = new TestCommand(commandHandler, "nested", CommandExecutionMode.Normal, () =>
            {
                nestedCommandThreadId = Thread.CurrentThread.ManagedThreadId;
                nestedCommandExecuted = true;
            });
            commandHandler.RegisterCommand(nestedCommand.Command);
            var parentCommand = new TestCommand(commandHandler, "cmd1", CommandExecutionMode.Normal, () =>
            {
                parentCommandThreadId = Thread.CurrentThread.ManagedThreadId;
                commandHandler.InvokeSyntax(",nested");
            });
            commandHandler.RegisterCommand(parentCommand.Command);

            commandHandler.InvokeSyntax(",cmd1");
            parentCommand.WaitForInitialization();
            nestedCommand.WaitForAdditionalAction();

            nestedCommandExecuted.Should().BeTrue();
            commandHandler.RunningCommands.Select(c => c.Name).Should().Contain("cmd1");
            commandHandler.RunningCommands.Select(c => c.Name).Should().NotContain("nested");
            nestedCommandThreadId.Should().NotBe(-1);
            parentCommandThreadId.Should().NotBe(-1);
            nestedCommandThreadId.Should().Be(parentCommandThreadId);

            nestedCommand.Finish();
            parentCommand.Finish();
        }

        [TestMethod]
        public void Can_execute_nested_Normal_command_from_Direct_parent()
        {
            var nestedCommandExecuted = false;
            var nestedCommand = new TestCommand(commandHandler, "nested", CommandExecutionMode.Normal,
                () =>
                {
                    nestedCommandExecuted = true;
                    commandHandler.RunningCommands.Select(c => c.Name).Should().Contain("parent");
                    commandHandler.RunningCommands.Select(c => c.Name).Should().NotContain("nested");
                });
            commandHandler.RegisterCommand(nestedCommand.Command);
            var parentCommand = new TestCommand(commandHandler, "parent", CommandExecutionMode.Direct,
                () => commandHandler.InvokeSyntax(",nested"));
            commandHandler.RegisterCommand(parentCommand.Command);

            nestedCommand.Finish();
            parentCommand.Finish();
            commandHandler.InvokeSyntax(",parent");

            nestedCommandExecuted.Should().BeTrue();
        }

        [TestMethod]
        public void Can_execute_nested_command_When_AlwaysParallel()
        {
            var nestedCommandExecuted = false;
            var nestedCommand = new TestCommand(commandHandler, "nested", CommandExecutionMode.AlwaysParallel, () => nestedCommandExecuted = true);
            commandHandler.RegisterCommand(nestedCommand.Command);
            var command = new TestCommand(commandHandler, "cmd1", () => commandHandler.InvokeSyntax(",nested"));
            commandHandler.RegisterCommand(command.Command);

            nestedCommand.Reset();
            command.Reset();
            commandHandler.InvokeSyntax(",cmd1");
            command.WaitForInitialization();
            nestedCommand.WaitForInitialization();
            nestedCommand.WaitForAdditionalAction();

            nestedCommandExecuted.Should().BeTrue();
            commandHandler.RunningCommands.Select(c => c.Name).Should().Contain("cmd1");
            commandHandler.RunningCommands.Select(c => c.Name).Should().Contain("nested");

            nestedCommand.Finish();
            command.Finish();
            nestedCommand.WaitForFinished();
            command.WaitForFinished();
        }

        [TestMethod]
        public void Can_list_Normal_commands_executing_Normal_nested_command_multipletimes()
        {
            var nestedCommand = new TestCommand(commandHandler, "nested", CommandExecutionMode.Normal, () => { });
            commandHandler.RegisterCommand(nestedCommand.Command);
            var command = new TestCommand(commandHandler, "cmd1", () => commandHandler.InvokeSyntax(",nested"));
            commandHandler.RegisterCommand(command.Command);

            for (int i = 0; i < 100; i++)
            {
                nestedCommand.Reset();
                command.Reset();

                commandHandler.InvokeSyntax(",cmd1");
                nestedCommand.WaitForAdditionalAction();

                commandHandler.RunningCommands.Select(x => x.Name).Should().Contain("cmd1");
                commandHandler.RunningCommands.Select(x => x.Name).Should().NotContain("nested");

                nestedCommand.Finish();
                command.Finish();
                command.WaitForFinished();

                commandHandler.RunningCommands.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void Cannot_execute_same_commands_in_parallel()
        {
            int executionCount = 0;

            var command = new TestCommand(commandHandler, "cmd1", () => executionCount++);
            commandHandler.RegisterCommand(command.Command);
            commandHandler.InvokeSyntax(",cmd1");
            commandHandler.InvokeSyntax(",cmd1");

            command.Finish();
            command.WaitForFinished();

            executionCount.Should().Be(1);
        }

        [TestMethod]
        public void Can_execute_Direct_command_on_caller_thread()
        {
            int commandThread = -1;
            var command = new TestCommand(commandHandler, "cmd1", CommandExecutionMode.Direct, () => commandThread = Thread.CurrentThread.ManagedThreadId);
            commandHandler.RegisterCommand(command.Command);
            command.Finish();
            commandHandler.InvokeSyntax(",cmd1");
            command.WaitForAdditionalAction();

            commandThread.Should().Be(Thread.CurrentThread.ManagedThreadId);
        }

        [TestMethod]
        public void Can_terminate_Normal_command_with_custom_CancellationTokenSource()
        {
            var command = new TestCommand(commandHandler, "cmd1", CommandExecutionMode.Normal, () => { DoSomeCancellableAction(); });
            commandHandler.RegisterCommand(command.Command);
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            commandHandler.InvokeSyntax(",cmd1", null, cancellationTokenSource);
            cancellationTokenSource.Cancel();

            command.Finish();

            command.WaitForFinished().Should().BeTrue();
            commandHandler.RunningCommands.Should().BeEmpty();
        }

        [TestMethod]
        public void Specific_Terminate_waits_for_command_termination()
        {
            var commandStartedEvent = new AutoResetEvent(false);
            bool commandEndReached = false;

            var command = new Command("cmd1", executionMode: CommandExecutionMode.Normal, commandAction: () =>
            {
                commandStartedEvent.Set();

                try
                {
                    DoSomeCancellableAction();
                }
                finally
                {
                    Thread.Sleep(10);
                    commandEndReached = true;
                }
            });

            commandHandler.RegisterCommand(command);
            commandHandler.Invoke("cmd1");

            commandStartedEvent.AssertWaitOneSuccess();
            commandHandler.Terminate("cmd1");

            var result = commandEndReached;
            result.Should().BeTrue();
        }

        [TestMethod]
        public void Can_terminate_Direct_command_with_custom_CancellationTokenSource()
        {
            var command = new TestCommand(commandHandler, "cmd1", CommandExecutionMode.Direct, () => { DoSomeCancellableAction(); });
            commandHandler.RegisterCommand(command.Command);
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            // ReSharper disable once MethodSupportsCancellation
            Task.Run(() => commandHandler.InvokeSyntax(",cmd1", null, cancellationTokenSource));

            command.Finish();
            cancellationTokenSource.Cancel();

            command.WaitForFinished().Should().BeTrue();
            commandHandler.RunningCommands.Should().BeEmpty();
        }

        [TestMethod]
        public void Can_call_Terminate_for_Direct_command_without_custom_CancellationTokenSource()
        {
            var command = new TestCommand(commandHandler, "cmd1", CommandExecutionMode.Direct, () => { });
            commandHandler.RegisterCommand(command.Command);

            // ReSharper disable once MethodSupportsCancellation
            Task.Run(() => commandHandler.InvokeSyntax(",cmd1"));
            command.WaitForAdditionalAction();

            commandHandler.BeginTerminate();

            commandHandler.CommandNames.Should().Contain(",cmd1", "Direct command without custom cancellation token cannot be terminated, to support special commands like ,terminate.");

            command.Finish();
        }

        [TestMethod]
        public void Non_specific_Terminate_does_not_terminate_background_commands()
        {
            var command = new TestCommand(commandHandler, "backgroundcmd", CommandExecutionMode.Background, () =>
            {
                DoSomeCancellableAction();
            });
            commandHandler.RegisterCommand(command.Command);

            commandHandler.InvokeSyntax(",backgroundcmd");
            command.WaitForInitialization();
            command.Finish();
            commandHandler.BeginTerminate();

            command.WaitForFinished(TimeSpan.FromMilliseconds(100)).Should()
                .BeFalse("background command should still run after non-specific terminate");
            commandHandler.RunningCommands.Select(x => x.Name).Should().Contain("backgroundcmd");
        }

        [TestMethod]
        public void Specific_Terminate_terminates_background_command()
        {
            var command = new TestCommand(commandHandler, "backgroundcmd", CommandExecutionMode.Background, () =>
            {
                DoSomeCancellableAction();
            });
            commandHandler.RegisterCommand(command.Command);

            commandHandler.InvokeSyntax(",backgroundcmd");
            command.WaitForInitialization();
            command.Finish();
            commandHandler.Terminate("backgroundcmd");

            command.WaitForFinished(TimeSpan.FromMilliseconds(100)).Should()
                .BeTrue("background command didn't finish");
            commandHandler.RunningCommands.Select(x => x.Name).Should().NotContain("backgroundcmd");
        }

        [TestMethod]
        public void Invoke_can_override_command_execution_mode()
        {
            int commandThreadId = -1;

            var command = new TestCommand(commandHandler, "backgroundcmd", CommandExecutionMode.Background, () =>
                {
                    commandThreadId = Thread.CurrentThread.ManagedThreadId;
                });
            commandHandler.RegisterCommand(command.Command);

            commandHandler.Invoke("backgroundcmd", CommandExecutionMode.Direct);

            commandThreadId.Should().Be(Thread.CurrentThread.ManagedThreadId);
        }

        [TestMethod]
        public void TerminateAll_terminates_background_commands()
        {
            var command = new TestCommand(commandHandler, "backgroundcmd", CommandExecutionMode.Background, () =>
            {
                DoSomeCancellableAction();
            });
            commandHandler.RegisterCommand(command.Command);

            commandHandler.InvokeSyntax(",backgroundcmd");
            command.WaitForInitialization();
            command.Finish();
            commandHandler.TerminateAll();

            command.WaitForFinished(TimeSpan.FromMilliseconds(100)).Should()
                .BeTrue("background command should not run after non-specific terminate all");
            commandHandler.RunningCommands.Select(x => x.Name).Should().NotContain("backgroundcmd");
        }

        [TestMethod]
        public void TerminateAll_terminates_normal_commands()
        {
            var command = new TestCommand(commandHandler, "normalcmd", CommandExecutionMode.Normal, () =>
            {
                DoSomeCancellableAction();
            });
            commandHandler.RegisterCommand(command.Command);

            commandHandler.InvokeSyntax(",normalcmd");
            command.WaitForInitialization();
            command.Finish();
            commandHandler.TerminateAll();

            command.WaitForFinished(TimeSpan.FromMilliseconds(100)).Should()
                .BeTrue("normal command should not run after non-specific terminate all");
            commandHandler.RunningCommands.Select(x => x.Name).Should().NotContain("normalcmd");
        }

        private sealed class TestCommand
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
}