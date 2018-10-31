using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Infusion.Packets;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.LegacyApi.Tests
{
    [TestClass]
    public class SpeechJournalAwaiterTests
    {
        private CancellationTokenSource cancellationTokenSource;
        private Cancellation cancellation;

        [TestInitialize]
        public void Initialize()
        {
            cancellationTokenSource = new CancellationTokenSource();
            cancellation = new Cancellation(() => cancellationTokenSource.Token);
        }

        [TestMethod]
        public void When_is_case_insensitive_by_default_when_waiting_and_already_in_journal()
        {
            var source = new SpeechJournalSource();
            var journal = new SpeechJournal(source);

            source.AddMessage("name", "cas InSeNsItIvE", new ObjectId(0), 0, (Color)0);

            bool executed = false;

            journal.When("insensitive", () => executed = true)
                .WaitAny(TimeSpan.FromMilliseconds(100));

            executed.Should().BeTrue();
        }

        [TestMethod]
        public void When_is_case_insensitive_by_default_when_waiting()
        {
            var source = new SpeechJournalSource();
            var journal = new SpeechJournal(source, null);

            bool executed = false;

            var task = Task.Run(() =>
            {
                journal.When("insensitive", () => executed = true)
                    .WaitAny(TimeSpan.FromMilliseconds(250));
            });
            Thread.Sleep(25);
            source.AddMessage("name", "InSeNsItIvE", new ObjectId(0), 0, (Color)0);

            task.Wait();

            executed.Should().BeTrue();
        }

        [TestMethod]
        public void Can_await_one_condition()
        {
            var executed = false;
            var initializedEvent = new AutoResetEvent(false);
            var awaiter = new SpeechJournalAwaiter(cancellation);

            var task = Task.Run(() =>
            {
                awaiter.When("test word", entry => executed = true);
                initializedEvent.Set();
                awaiter.WaitAny();
            });

            initializedEvent.WaitOne(100);
            awaiter.ReceiveJournalEntry(new JournalEntry(0, "test", "test word", new ObjectId(1234), (ModelId) 4321, (Color)0));
            task.Wait();

            executed.Should().Be(true);
        }

        [TestMethod]
        public void Can_await_two_conditions()
        {
            var firstConditionExecuted = false;
            var secondConditionExecuted = false;
            var initializedEvent = new AutoResetEvent(false);
            var awaiter = new SpeechJournalAwaiter(cancellation);

            var task = Task.Run(() =>
            {
                awaiter
                    .When("word1", entry => firstConditionExecuted = true)
                    .When("word2", entry => secondConditionExecuted = true);
                initializedEvent.Set();
                awaiter.WaitAny();
            });

            initializedEvent.WaitOne(100);
            awaiter.ReceiveJournalEntry(new JournalEntry(0, "TestName", "somethingsomething word2 somethingsomething", new ObjectId(1234),
                (ModelId) 4321, (Color)0));

            task.Wait(100);

            firstConditionExecuted.Should().BeFalse();
            secondConditionExecuted.Should().BeTrue();
        }

        [TestMethod]
        public void Can_await_multiple_words()
        {
            var executed = false;
            var initializedEvent = new AutoResetEvent(false);
            var awaiter = new SpeechJournalAwaiter(cancellation);

            var task = Task.Run(() =>
            {
                awaiter
                    .When(new[] {"word1", "test word", "word2"}, entry => executed = true);
                initializedEvent.Set();
                awaiter.WaitAny();
            });

            initializedEvent.WaitOne(100);
            awaiter.ReceiveJournalEntry(new JournalEntry(0, "test", "test word", new ObjectId(1234), (ModelId) 4321, (Color)0));
            task.Wait(100);

            executed.Should().Be(true);
        }

        [TestMethod]
        public void Can_cancell_awaiting()
        {
            var initializedEvent = new AutoResetEvent(false);
            var awaiter = new SpeechJournalAwaiter(cancellation);

            var task = Task.Run(() =>
            {
                Action action = () =>
                {
                    initializedEvent.Set();
                    awaiter.WaitAny();
                };
                action.ShouldThrow<OperationCanceledException>();
            });

            initializedEvent.WaitOne(100);
            cancellationTokenSource.Cancel();
            task.Wait(1000).Should().BeTrue();
        }

        [TestMethod]
        public void Can_execute_action_when_awaiting_timeouts()
        {
            bool executed = false;
            var awaiter = new SpeechJournalAwaiter(cancellation);

            awaiter.WhenTimeout(() => executed = true)
                .WaitAny(TimeSpan.FromMilliseconds(10));

            executed.Should().BeTrue();
        }

        [TestMethod]
        public void Throws_exception_When_awaiting_timeouts_and_no_timeout_action()
        {
            var source = new SpeechJournalSource();
            var awaiter = new SpeechJournalAwaiter(cancellation, source);
            source.AddMessage("name1", "message1", 0x12345, 0x1234, (Color)0);

            ((Action) (() => awaiter.WaitAny(TimeSpan.FromMilliseconds(10))))
                .ShouldThrow<TimeoutException>();
        }
    }
}