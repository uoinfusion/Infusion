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

            journal.AwaitingStarted.WaitOne(100).Should().BeTrue();

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

                journal.AwaitingStarted.WaitOne(100).Should().BeTrue();

                source.Publish(new SpeechRequestedEvent("some message"));

                executedEvent.WaitOne(100).Should().BeTrue("one of when actions has to be executed before timeout");

                receivedQuest.Should().BeNull();
                receivedSpeech.Should().NotBeNull();
                receivedSpeech.Message.Should().Be("some message");
            });
        }

        [TestMethod]
        public void First_unconditional_when_is_executed()
        {
            bool conditionalWhenExecuted = false;
            bool unconditionalWhenExecuted = false;
            var source = new EventJournalSource();
            var journal = new EventJournal(source);

            var task = Task.Run(() =>
            {
                journal
                    .When<SpeechRequestedEvent>(e =>
                    {
                        unconditionalWhenExecuted = true;
                    })
                    .When<SpeechRequestedEvent>(e =>
                    {
                        // another unconditional when, that cannot be executed
                    })
                    .When<SpeechRequestedEvent>(e => e.Message == "I handle this message", e =>
                    {
                        conditionalWhenExecuted = true;
                    })
                    .WaitAny();
            });

            journal.AwaitingStarted.WaitOne(TimeSpan.FromMilliseconds(100)).Should().BeTrue();

            source.Publish(new SpeechRequestedEvent("I handle this message"));

            task.Wait(TimeSpan.FromMilliseconds(100)).Should().BeTrue();

            conditionalWhenExecuted.Should().BeFalse();
            unconditionalWhenExecuted.Should().BeTrue();
        }

        [TestMethod]
        public void First_satisfied_conditional_when_is_executed()
        {
            bool conditionalWhenExecuted = false;
            bool notSatisfiedConditionalWhen = false;
            bool unconditionalWhenExecuted = false;
            var source = new EventJournalSource();
            var journal = new EventJournal(source);

            var task = Task.Run(() =>
            {
                journal
                    .When<SpeechRequestedEvent>(e => e.Message == "I don't handle this message", e =>
                    {
                        notSatisfiedConditionalWhen = true;
                    })
                    .When<SpeechRequestedEvent>(e => e.Message == "I handle this message", e =>
                    {
                        conditionalWhenExecuted = true;
                    })
                    .When<SpeechRequestedEvent>(e =>
                    {
                        unconditionalWhenExecuted = true;
                    })
                    .WaitAny();
            });

            journal.AwaitingStarted.WaitOne(TimeSpan.FromMilliseconds(100)).Should().BeTrue();

            source.Publish(new SpeechRequestedEvent("something else than I refuse this message"));

            task.Wait(TimeSpan.FromMilliseconds(100)).Should().BeTrue();

            unconditionalWhenExecuted.Should().BeTrue();
            conditionalWhenExecuted.Should().BeFalse();
            notSatisfiedConditionalWhen.Should().BeFalse();
        }


        [TestMethod]
        public void When_can_refuse_to_handle_message()
        {
            bool conditionalWhenExecuted = false;
            bool unconditionalWhenExecuted = false;
            var source = new EventJournalSource();
            var journal = new EventJournal(source);

            var task = Task.Run(() =>
            {
                journal
                    .When<SpeechRequestedEvent>(e => e.Message == "I refuse this message", e =>
                    {
                        conditionalWhenExecuted = true;
                    })
                    .When<SpeechRequestedEvent>(e =>
                    {
                        unconditionalWhenExecuted = true;
                    })
                    .WaitAny();
            });

            journal.AwaitingStarted.WaitOne(TimeSpan.FromMilliseconds(100)).Should().BeTrue();

            source.Publish(new SpeechRequestedEvent("something else than I refuse this message"));

            task.Wait(TimeSpan.FromMilliseconds(100)).Should().BeTrue();

            conditionalWhenExecuted.Should().BeFalse();
            unconditionalWhenExecuted.Should().BeTrue();
        }

        [TestMethod]
        public void When_WaitAny_Then_executes_only_once_when_action_of_event_received_first()
        {
            ConcurrencyTester.Run(() =>
            {
                var source = new TestEventJournalSource();
                var journal = new EventJournal(source);
                SpeechRequestedEvent receivedSpeech = null;

                var task = Task.Run(() =>
                {
                    journal
                        .When<SpeechRequestedEvent>(e =>
                        {
                            receivedSpeech = e;
                        })
                        .WaitAny();
                });

                journal.AwaitingStarted.WaitOne(100).Should().BeTrue();

                var questArrowEvent = new SpeechRequestedEvent("first message");
                source.Publish(questArrowEvent);

                var speechEvent = new SpeechRequestedEvent("second message");
                source.Publish(speechEvent);

                source.SignalEventReceived(questArrowEvent);
                source.SignalEventReceived(speechEvent);

                task.Wait(100).Should().BeTrue();

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
            ConcurrencyTester.Run(() =>
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

                initializedEvent.WaitOne(100).Should()
                    .BeTrue("awaiting should start immediatelly, false means a timeout");
                Thread.Yield();

                cancellationTokenSource.Cancel();

                task.Wait(TimeSpan.FromMilliseconds(100)).Should()
                    .BeTrue("false means timeout - tested task was not executed in time");
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
                var journal = new EventJournal(source, () => cancellationTokenSource.Token);
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

                journal.AwaitingStarted.WaitOne(100).Should().BeTrue();

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

        [TestMethod]
        public void Can_handle_all_unprocessed_events()
        {
            var source = new EventJournalSource();
            var journal = new EventJournal(source);
            bool speechRequestedEventHandled = false;
            bool questArrowEventHandled = false;

            source.Publish(new SpeechRequestedEvent("some text"));
            source.Publish(new QuestArrowEvent(true, new Location2D(123, 321)));

            journal
                .When<SpeechRequestedEvent>(e => speechRequestedEventHandled = true)
                .When<QuestArrowEvent>(e => questArrowEventHandled = true)
                .All();

            speechRequestedEventHandled.Should().BeTrue();
            questArrowEventHandled.Should().BeTrue();
        }

        [TestMethod]
        public void Can_handle_all_unprocessed_events_with_multiple_subscriptions_to_one_event()
        {
            var source = new EventJournalSource();
            var journal = new EventJournal(source);
            bool firstHandlerInvoked = false;
            bool secondHandlerInvoked = false;

            source.Publish(new SpeechRequestedEvent("some text"));

            journal
                .When<SpeechRequestedEvent>(e => firstHandlerInvoked = true)
                .When<SpeechRequestedEvent>(e => secondHandlerInvoked = true)
                .All();

            firstHandlerInvoked.Should().BeTrue();
            secondHandlerInvoked.Should().BeTrue();
        }

        [TestMethod]
        public void All_event_handling_doesnt_handle_handled_events()
        {
            var source = new EventJournalSource();
            var journal = new EventJournal(source);
            bool handlerInvoked = false;

            source.Publish(new SpeechRequestedEvent("some text"));

            journal
                .When<SpeechRequestedEvent>(e => handlerInvoked = true)
                .All();

            handlerInvoked.Should().BeTrue();

            handlerInvoked = false;
            journal
                .When<SpeechRequestedEvent>(e => handlerInvoked = true)
                .All();

            handlerInvoked.Should().BeFalse();
        }

        [TestMethod]
        public void All_event_handling_can_be_invoked_multiple()
        {
            var source = new EventJournalSource();
            var journal = new EventJournal(source);
            bool handlerInvoked = false;

            source.Publish(new SpeechRequestedEvent("some text"));

            journal
                .When<SpeechRequestedEvent>(e => handlerInvoked = true)
                .All();

            handlerInvoked.Should().BeTrue();

            handlerInvoked = false;
            source.Publish(new SpeechRequestedEvent("some text"));
            journal
                .When<SpeechRequestedEvent>(e => handlerInvoked = true)
                .All();

            handlerInvoked.Should().BeTrue();
        }

        [TestMethod]
        public void All_events_handling_respects_when_predicate()
        {
            var source = new EventJournalSource();
            var journal = new EventJournal(source);
            bool handlerWithFalsePredicate = false;
            bool handlerWithTruePredicate = false;

            source.Publish(new SpeechRequestedEvent("some text"));

            journal
                .When<SpeechRequestedEvent>(e => e.Message == "something else", e => handlerWithFalsePredicate = true)
                .When<SpeechRequestedEvent>(e => e.Message == "some text", e => handlerWithTruePredicate = true)
                .All();

            handlerWithFalsePredicate.Should().BeFalse();
            handlerWithTruePredicate.Should().BeTrue();
        }

        [TestMethod]
        public void Can_handle_event_published_during_handling_all_unprocessed_events()
        {
            ConcurrencyTester.Run(() =>
            {
                var source = new EventJournalSource();
                var journal = new EventJournal(source);
                bool handlerInvoked = false;
                var waitInsideEventHandler = new AutoResetEvent(false);

                source.Publish(new SpeechRequestedEvent("some text"));

                var task = Task.Run(() =>
                {
                    journal
                        .When<SpeechRequestedEvent>(e =>
                        {
                            waitInsideEventHandler.WaitOne();
                        })
                        .All();
                });

                journal.AwaitingStarted.WaitOne(100).Should().BeTrue();

                source.Publish(new SpeechRequestedEvent("other text"));
                waitInsideEventHandler.Set();
                task.Wait(100).Should().BeTrue();

                journal
                    .When<SpeechRequestedEvent>(e => handlerInvoked = true)
                    .All();

                handlerInvoked.Should().BeTrue();
            });
        }

        [TestMethod]
        public void Can_delete_journal_and_All_handles_next_event()
        {
            var source = new EventJournalSource();
            var journal = new EventJournal(source);

            bool eventBeforeDeleteHandled = false;
            bool eventAfterDeleteHandled = false;

            source.Publish(new SpeechRequestedEvent("event that should be deleted"));
            journal.Delete();
            source.Publish(new QuestArrowEvent(true, new Location2D(123, 321)));

            journal.When<SpeechRequestedEvent>(e => eventBeforeDeleteHandled = true)
                .When<QuestArrowEvent>(e => eventAfterDeleteHandled = true)
                .All();

            eventBeforeDeleteHandled.Should().BeFalse();
            eventAfterDeleteHandled.Should().BeTrue();
        }

        [TestMethod]
        public void Can_cancel_All_handling()
        {
            var source = new EventJournalSource();
            var cancellationTokenSource = new CancellationTokenSource();
            var journal = new EventJournal(source, () => cancellationTokenSource.Token);

            var task = Task.Run(() =>
            {
                Action action = () =>
                {
                    while (true)
                    {
                        journal.When<SpeechRequestedEvent>(e => { })
                            .All();
                    }
                };

                action.ShouldThrow<OperationCanceledException>();
            });

            journal.AwaitingStarted.WaitOne(100).Should().BeTrue();

            cancellationTokenSource.Cancel();
            task.Wait(TimeSpan.FromMilliseconds(100)).Should()
                .BeTrue("false means timeout - tested task was not cancelled in time");
        }

        [TestMethod]
        public void Can_cancel_All_handling_of_many_events_with_noncancellable_handlers()
        {
            var source = new EventJournalSource();
            var cancellationTokenSource = new CancellationTokenSource();
            var journal = new EventJournal(source, () => cancellationTokenSource.Token);

            for (int i = 0; i < 10000; i++)
            {
                source.Publish(new SpeechRequestedEvent(i.ToString()));
            }

            var task = Task.Run(() =>
            {
                Action action = () =>
                {
                    journal.When<SpeechRequestedEvent>(e => { Thread.Sleep(25); })
                        .All();
                };

                action.ShouldThrow<OperationCanceledException>();
            });

            journal.AwaitingStarted.WaitOne(100).Should().BeTrue();

            cancellationTokenSource.Cancel();
            task.Wait(TimeSpan.FromMilliseconds(100)).Should()
                .BeTrue("false means timeout - tested task was not cancelled in time");
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
        public int MaximumCapacity => 128;
        public void GetherEvents(ICollection<IEvent> targetCollection, EventId minEventId, EventId maxEventId)
        {
            foreach (var ev in Events)
            {
                if (ev.Id > maxEventId)
                    return;

                if (ev.Id >= minEventId)
                    targetCollection.Add(ev.Event);
            }
        }

        public void SignalEventReceived(IEvent ev)
        {
            NewEventReceived?.Invoke(this, ev);
        }
    }
}
