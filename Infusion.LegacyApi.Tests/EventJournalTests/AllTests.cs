using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Infusion.LegacyApi.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.LegacyApi.Tests.EventJournalTests
{
    [TestClass]
    public class AllTests
    {
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
            var journal = new EventJournal(source, new Cancellation(() => cancellationTokenSource.Token));

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
            var journal = new EventJournal(source, new Cancellation(() => cancellationTokenSource.Token));

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
}
