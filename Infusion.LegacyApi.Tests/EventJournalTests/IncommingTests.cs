using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Infusion.LegacyApi.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.LegacyApi.Tests.EventJournalTests
{
    [TestClass]
    public class EventJournalIncommingTests
    {
        [TestMethod]
        public void Can_cancel_awaiting()
        {
            ConcurrencyTester.Run(() =>
            {
                var initializedEvent = new AutoResetEvent(false);
                var source = new EventJournalSource();
                var cancellationTokenSource = new CancellationTokenSource();
                var journal = new EventJournal(source, new Cancellation(() => cancellationTokenSource.Token));

                var task = Task.Run(() =>
                {
                    Action action = () =>
                    {
                        initializedEvent.Set();
                        journal
                            .When<QuestArrowEvent>(e => { })
                            .WaitAny();
                    };

                    action.ShouldThrow<OperationCanceledException>();
                });

                initializedEvent.AssertWaitOneSuccess();
                Thread.Yield();

                cancellationTokenSource.Cancel();

                task.AssertWaitFastSuccess();
            });
        }

        [TestMethod]
        public void Can_handle_incomming_events()
        {
            ConcurrencyTester.Run(() =>
            {
                var finishedEvent = new AutoResetEvent(false);
                int whenExecutedCount = 0;
                var source = new EventJournalSource();
                var cancellationTokenSource = new CancellationTokenSource();
                var journal = new EventJournal(source, new Cancellation(() => cancellationTokenSource.Token));
                var resultBuilder = new StringBuilder();

                var task = Task.Run(() =>
                {
                    Action testedAction = () =>
                    {
                        journal
                            .When<SpeechRequestedEvent>(e =>
                            {
                                resultBuilder.Append(e.Message);
                                whenExecutedCount++;
                                if (whenExecutedCount >= 3)
                                    finishedEvent.Set();
                            })
                            .Incomming();
                    };

                    testedAction.ShouldThrow<OperationCanceledException>();
                });

                journal.AwaitingStarted.AssertWaitOneSuccess();

                source.Publish(new SpeechRequestedEvent("message1"));
                source.Publish(new SpeechRequestedEvent("message2"));
                source.Publish(new SpeechRequestedEvent("message3"));

                finishedEvent.AssertWaitOneSuccess();

                cancellationTokenSource.Cancel();
                task.AssertWaitFastSuccess();

                resultBuilder.ToString().Should().Be("message1message2message3");
            });
        }

    }
}
