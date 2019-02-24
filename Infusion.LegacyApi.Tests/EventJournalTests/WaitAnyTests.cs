using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Infusion.LegacyApi.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.LegacyApi.Tests.EventJournalTests
{
    [TestClass]
    public class WaitAnyTests
    {
        [TestMethod]
        public void Can_await_one_event()
        {
            var source = new EventJournalSource();
            var journal = new EventJournal(source);
            var executedEvent = new AutoResetEvent(false);
            QuestArrowEvent receivedEvent = null;

            var x = journal.AwaitingStarted;
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
        public void Executes_only_once_when_action_of_event_received_first()
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

                task.Wait(100).Should().BeTrue();

                receivedSpeech.Should().NotBeNull();
                receivedSpeech.Message.Should().Be("first message");
            });
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
        public void Unprocessed_events_by_first_WaitAny_are_visible_to_second_WaitAny()
        {
            // It covers a case when script waits for an event and then has to wait for a second event.
            // The second event may arrive nearly at the same time as the first event.
            // There are two WaitAny calls in a single code block. There is just one action,
            // before the first WaitAny.
            // In other words, two consecutive calls to WaitAny have to process all events, no unhandled "holes"
            // are allowed.
            var source = new EventJournalSource();
            var journal = new EventJournal(source);
            string result = string.Empty;

            var task = Task.Run(() =>
            {
                journal
                    .When<ContainerOpenedEvent>(e => result = "y")
                    .When<SpeechReceivedEvent>(e => e.Speech.Message == "qwer", e => { result = "x"; })
                    .WaitAny();
            });

            journal.AwaitingStarted.WaitOne(100).Should().BeTrue();
            source.Publish(new SpeechReceivedEvent(new JournalEntry(1, "qwer", "qwer", 0, 0, (Color)0)));
            source.Publish(new SpeechReceivedEvent(new JournalEntry(1, "asdf", "asdf", 0, 0, (Color)0)));
            task.Wait(100).Should().BeTrue();
            result.Should().Be("x");


            result = "z";
            task = Task.Run(() =>
            {
                journal
                    .When<ContainerOpenedEvent>(e => result = "published after the second WaitAny started")
                    .When<SpeechReceivedEvent>(e => e.Speech.Message == "qwer", e => { result = "processed by the first WaitAny"; })
                    .When<SpeechReceivedEvent>(e => e.Speech.Message == "asdf", e => { result = "not processed by the first WaitAny"; })
                    .WaitAny();
            });

            journal.AwaitingStarted.WaitOne(100).Should().BeTrue();
            source.Publish(new ContainerOpenedEvent(1));

            task.Wait(100).Should().BeTrue();
            result.Should().Be("not processed by the first WaitAny");
        }

        [TestMethod]
        public void Can_handle_events_published_after_action_but_before_awaiting()
        {
            var canPublishEvent = new AutoResetEvent(false);
            var canAwaitEvent = new AutoResetEvent(false);
            var source = new EventJournalSource();
            var journal = new EventJournal(source);
            
            string handledMessage = string.Empty;
            var task = Task.Run(() =>
            {
                canPublishEvent.Set();
                canAwaitEvent.WaitOne(100).Should().BeTrue();

                journal.When<SpeechRequestedEvent>(ev => handledMessage = ev.Message)
                    .WaitAny(TimeSpan.FromMilliseconds(100));
            });

            source.NotifyAction();
            canPublishEvent.WaitOne(100).Should().BeTrue();
            source.Publish(new SpeechRequestedEvent("message1"));
            canAwaitEvent.Set();

            journal.AwaitingStarted.WaitOne(100).Should().BeTrue();
            source.Publish(new SpeechRequestedEvent("message2 - we don't want to get this one"));

            task.Wait(100).Should().BeTrue();

            handledMessage.Should().Be("message1");
        }

        [TestMethod]
        public void Can_use_same_journal_concurrently()
        {
            ConcurrencyTester.Run(() =>
            {
                var source = new EventJournalSource();
                var journal = new EventJournal(source);
                string task1Result = string.Empty;
                string task2Result = string.Empty;

                var task1 = Task.Run(() =>
                {
                    journal.When<SpeechRequestedEvent>(e => { task1Result = "SpeechRequestedEvent"; })
                        .WaitAny();
                });

                journal.AwaitingStarted.WaitOne(100).Should().BeTrue();

                var task2 = Task.Run(() =>
                {
                    journal.When<DialogBoxOpenedEvent>(e => { task2Result = "DialogBoxOpenedEvent"; })
                        .WaitAny();
                });

                journal.AwaitingStarted.WaitOne(100).Should().BeTrue();

                source.Publish(new SpeechRequestedEvent("test"));
                source.Publish(new DialogBoxOpenedEvent(new DialogBox(1, 1, "", null)));

                task1.Wait(100).Should().BeTrue();
                task2.Wait(100).Should().BeTrue();

                task1Result.Should().Be("SpeechRequestedEvent");
                task2Result.Should().Be("DialogBoxOpenedEvent");
            });
        }

    }
}
