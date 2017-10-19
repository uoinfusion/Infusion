using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Infusion.LegacyApi.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.LegacyApi.Tests
{
    [TestClass]
    public class EventJournalTests
    {
        [TestMethod]
        public void Can_await_one_event()
        {
            var source = new EventJournalSource();
            var journal = new EventJournal(source);
            var executedEvent = new AutoResetEvent(false);
            QuestArrowEvent receivedEvent = null;

            var task = Task.Run(() =>
            {
                journal.When<QuestArrowEvent>(e =>
                    {
                        receivedEvent = e;
                        executedEvent.Set();
                    })
                    .WaitAny();
            });

            Thread.Sleep(10);

            source.Publish(new QuestArrowEvent(true, new Location2D(123, 321)));

            executedEvent.WaitOne(100).Should().BeTrue();

            receivedEvent.Should().NotBeNull();
            receivedEvent.Active.Should().BeTrue();
            receivedEvent.Location.Should().Be(new Location2D(123, 321));
        }

        [TestMethod]
        public void When_awaiting_any_of_two_events_Then_executes_when_action_of_first_received_event()
        {
            ConcurrencyTester.Run(() =>
            {
                var source = new EventJournalSource();
                var journal = new EventJournal(source);
                var executedEvent = new AutoResetEvent(false);
                QuestArrowEvent receivedQuest = null;
                SpeechRequestedEvent receivedSpeech = null;

                var task = Task.Run(() =>
                {
                    journal
                        .When<QuestArrowEvent>(e =>
                        {
                            receivedQuest = e;
                            executedEvent.Set();
                        })
                        .When<SpeechRequestedEvent>(e =>
                        {
                            receivedSpeech = e;
                            executedEvent.Set();
                        })
                        .WaitAny();
                });

                Thread.Sleep(10);

                source.Publish(new SpeechRequestedEvent("some message"));

                executedEvent.WaitOne(100).Should().BeTrue("one of when actions has to be executed before timeout");

                receivedQuest.Should().BeNull();
                receivedSpeech.Should().NotBeNull();
                receivedSpeech.Message.Should().Be("some message");
            });
        }

        [TestMethod]
        public void When_awaiting_Then_executes_only_once_when_action_of_event_received_first()
        {
            ConcurrencyTester.Run(() =>
            {
                var source = new TestEventJournalSource();
                var journal = new EventJournal(source);
                var executedEvent = new AutoResetEvent(false);
                SpeechRequestedEvent receivedSpeech = null;

                var task = Task.Run(() =>
                {
                    journal
                        .When<SpeechRequestedEvent>(e =>
                        {
                            receivedSpeech = e;
                            executedEvent.Set();
                        })
                        .WaitAny();
                });

                Thread.Sleep(10);

                var questArrowEvent = new SpeechRequestedEvent("first message");
                source.Publish(questArrowEvent);

                var speechEvent = new SpeechRequestedEvent("second message");
                source.Publish(speechEvent);

                source.SignaleEventReceived(questArrowEvent);
                source.SignaleEventReceived(speechEvent);

                executedEvent.WaitOne(100).Should().BeTrue();

                receivedSpeech.Should().NotBeNull();
                receivedSpeech.Message.Should().Be("first message");
            });
        }

        [TestMethod]
        public void Can_enumerate_all_journal_events()
        {
            var source = new EventJournalSource();
            var journal = new EventJournal(source);

            source.Publish(new CommandRequestedEvent(",somesyntax"));
            source.Publish(new QuestArrowEvent(true, new Location2D(123, 321)));

            journal.Count().Should().Be(2);
            journal.First().Should().BeOfType<CommandRequestedEvent>()
                .Which.InvocationSyntax.Should().Be(",somesyntax");
            journal.Last().Should().BeOfType<QuestArrowEvent>()
                .Which.Location.Should().Be(new Location2D(123, 321));
        }

        [TestMethod]
        public void Cannot_see_events_published_before_journal_creation()
        {
            var source = new EventJournalSource();
            source.Publish(new CommandRequestedEvent(",before1"));
            source.Publish(new CommandRequestedEvent(",before2"));

            var journal = new EventJournal(source);
            source.Publish(new CommandRequestedEvent(",after"));

            journal.Count().Should().Be(1, "1 event was added to event source before journal creation");
            journal.Single().Should().BeOfType<CommandRequestedEvent>()
                .Which.InvocationSyntax.Should().Be(",after");
        }

        [TestMethod]
        public void Can_timeout_when_none_of_awaited_events_published()
        {
            var source = new EventJournalSource();
            var journal = new EventJournal(source);

            Action actionThrowingTimeoutException = () =>
            {
                journal
                    .When<QuestArrowEvent>(e => { })
                    .WaitAny(TimeSpan.FromMilliseconds(10));
            };

            actionThrowingTimeoutException.ShouldThrow<TimeoutException>();
        }

        [TestMethod]
        public void Can_cancel_awaiting()
        {
            var initializedEvent = new AutoResetEvent(false);
            var source = new EventJournalSource();
            var cancellationTokenSource = new CancellationTokenSource();
            var journal = new EventJournal(source, () => cancellationTokenSource.Token);

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

            initializedEvent.WaitOne(100).Should().BeTrue("awaiting should start immediatelly, false means a timeout");

            cancellationTokenSource.Cancel();

            task.Wait(TimeSpan.FromMilliseconds(100)).Should().BeTrue("false means timeout - tested task was not executed in time");
        }

        [TestMethod]
        public void Can_handle_incomming_events()
        {
            ConcurrencyTester.Run(() =>
            {
                var initializedEvent = new AutoResetEvent(false);
                var finishedEvent = new AutoResetEvent(false);
                int whenExecutedCount = 0;
                var source = new EventJournalSource();
                var cancellationTokenSource = new CancellationTokenSource();
                var journal = new EventJournal(source, () => cancellationTokenSource.Token);
                var resultBuilder = new StringBuilder();

                var task = Task.Run(() =>
                {
                    Action testedAction = () =>
                    {
                        initializedEvent.Set();
                        journal
                            .When<SpeechRequestedEvent>(e =>
                            {
                                resultBuilder.Append(e.Message);
                                whenExecutedCount++;
                                if (whenExecutedCount >= 3)
                                    finishedEvent.Set();
                            })
                            .HandleIncomming();
                    };

                    testedAction.ShouldThrow<OperationCanceledException>();
                });

                initializedEvent.WaitOne(100).Should()
                    .BeTrue("task with HandleIncomming should start immediatelly, false means a suspicious timeout");
                Thread.Sleep(1);

                source.Publish(new SpeechRequestedEvent("message1"));
                source.Publish(new SpeechRequestedEvent("message2"));
                source.Publish(new SpeechRequestedEvent("message3"));

                finishedEvent.WaitOne(TimeSpan.FromMilliseconds(100)).Should()
                    .BeTrue("the test didn't finished in time");

                cancellationTokenSource.Cancel();
                task.Wait(TimeSpan.FromMilliseconds(100)).Should()
                    .BeTrue("false means timeout - tested task was not executed in time");

                resultBuilder.ToString().Should().Be("message1message2message3");
            });
        }
    }

    internal class TestEventJournalSource : IEventJournalSource
    {
        private readonly EventJournalSource source = new EventJournalSource();
        public event EventHandler<IEvent> NewEventReceived;

        public void Publish(IEvent ev)
        {
            source.Publish(ev);
        }

        public IEnumerable<OrderedEvent> Events => source.Events;
        public EventId LastEventId => source.LastEventId;

        public void SignaleEventReceived(IEvent ev)
        {
            NewEventReceived?.Invoke(this, ev);
        }
    }
}
