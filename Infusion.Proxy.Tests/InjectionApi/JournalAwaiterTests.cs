using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Infusion.Packets;
using Infusion.Proxy.LegacyApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.Proxy.Tests.InjectionApi
{
    [TestClass]
    public class JournalAwaiterTests
    {
        private CancellationTokenSource cancellationTokenSource;
        private Func<CancellationToken?> cancellationTokenProvider;

        [TestInitialize]
        public void Initialize()
        {
            cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenProvider = () => cancellationTokenSource.Token;
        }

        [TestMethod]
        public void Can_await_one_condition()
        {
            var executed = false;
            var initializedEvent = new AutoResetEvent(false);
            var awaiter = new JournalAwaiter(cancellationTokenProvider);

            var task = Task.Run(() =>
            {
                awaiter.When("test word", entry => executed = true);
                initializedEvent.Set();
                awaiter.WaitAny();
            });

            initializedEvent.WaitOne(100);
            awaiter.ReceiveJournalEntry(new JournalEntry("test", "test word", 1234, (ModelId) 4321));
            task.Wait();

            executed.Should().Be(true);
        }

        [TestMethod]
        public void Can_await_two_conditions()
        {
            var firstConditionExecuted = false;
            var secondConditionExecuted = false;
            var initializedEvent = new AutoResetEvent(false);
            var awaiter = new JournalAwaiter(cancellationTokenProvider);

            var task = Task.Run(() =>
            {
                awaiter
                    .When("word1", entry => firstConditionExecuted = true)
                    .When("word2", entry => secondConditionExecuted = true);
                initializedEvent.Set();
                awaiter.WaitAny();
            });

            initializedEvent.WaitOne(100);
            awaiter.ReceiveJournalEntry(new JournalEntry("TestName", "somethingsomething word2 somethingsomething", 1234,
                (ModelId) 4321));

            task.Wait(100);

            firstConditionExecuted.Should().BeFalse();
            secondConditionExecuted.Should().BeTrue();
        }

        [TestMethod]
        public void Can_await_multiple_words()
        {
            var executed = false;
            var initializedEvent = new AutoResetEvent(false);
            var awaiter = new JournalAwaiter(cancellationTokenProvider);

            var task = Task.Run(() =>
            {
                awaiter
                    .When(new[] {"word1", "test word", "word2"}, entry => executed = true);
                initializedEvent.Set();
                awaiter.WaitAny();
            });

            initializedEvent.WaitOne(100);
            awaiter.ReceiveJournalEntry(new JournalEntry("test", "test word", 1234, (ModelId) 4321));
            task.Wait(100);

            executed.Should().Be(true);
        }

        [TestMethod]
        public void Can_cancell_awaiting()
        {
            var initializedEvent = new AutoResetEvent(false);
            var awaiter = new JournalAwaiter(() => cancellationTokenSource.Token);

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
            var awaiter = new JournalAwaiter(() => cancellationTokenSource.Token);

            awaiter.WhenTimeout(() => executed = true)
                .WaitAny(TimeSpan.FromMilliseconds(100));

            executed.Should().BeTrue();
        }
    }
}