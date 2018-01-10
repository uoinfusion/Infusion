using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Infusion.Packets;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.LegacyApi.Tests
{
    [TestClass]
    public class SpeechJournalTests
    {
        [TestMethod]
        public void Contains_is_case_insensitive_by_default()
        {
            var source = new JournalSource();
            var journal = new SpeechJournal(source, null);

            source.AddMessage("name", "this is an AfK check", new ObjectId(0), 0);

            journal.Contains("afk").Should().BeTrue();
        }

        [TestMethod]
        public void Contains_checks_whole_message_including_speaker_name()
        {
            var source = new JournalSource();
            var journal = new SpeechJournal(source, null);

            source.AddMessage("name", "this is an AfK check", new ObjectId(0), 0);

            journal.Contains("name: this").Should().BeTrue();
        }

        [TestMethod]
        public void Contains_returns_false_if_journal_doesnt_contains_string()
        {
            var source = new JournalSource();
            var journal = new SpeechJournal(source, null);

            source.AddMessage("name", "this is an AfK check", new ObjectId(0), 0);

            journal.Contains("and now for completely something else").Should().BeFalse();
        }

        [TestMethod]
        public void Can_await_entries_received_between_lastaction_and_WaitAny()
        {
            bool executed = false;
            var source = new JournalSource();
            var journal = new SpeechJournal(source, null);

            source.AddMessage("name", "before last action", new ObjectId(0), 0);
            source.NotifyAction();
            source.AddMessage("name", "after last action", new ObjectId(0), 0);

            journal.When("after last action", () => executed = true).WaitAny(TimeSpan.FromMilliseconds(100));

            executed.Should().BeTrue();
        }

        [TestMethod]
        public void Doesnt_await_entries_received_before_lastaction()
        {
            bool executed = false;
            bool timeoutExecuted = false;
            var source = new JournalSource();
            var journal = new SpeechJournal(source, null);

            source.AddMessage("name", "before last action", new ObjectId(0), 0);
            source.NotifyAction();
            source.AddMessage("name", "after last action", new ObjectId(0), 0);

            journal.When("before last action", () => executed = true).WhenTimeout(() => timeoutExecuted = true).WaitAny(TimeSpan.FromMilliseconds(10));

            timeoutExecuted.Should().BeTrue();
            executed.Should().BeFalse();
        }

        [TestMethod]
        public void Lastaction_is_not_affected_by_actions_on_another_thread()
        {
            var source = new JournalSource();
            var scriptJournal = new SpeechJournal(source);
            var nextScriptStep = new AutoResetEvent(false);
            var nextOtherThreadStep = new AutoResetEvent(false);
            bool message1Received = false;
            bool message2Received = false;

            var scriptTask = Task.Run(() =>
            {
                source.NotifyAction();

                nextOtherThreadStep.Set();
                nextScriptStep.WaitOne();

                scriptJournal
                    .When("message 1", () => message1Received = true)
                    .When("message 2", () => message2Received = true)
                    .WaitAny();
            });

            nextOtherThreadStep.WaitOne();

            source.AddMessage("name", "message 1", 0, 0);
            source.NotifyAction();
            source.AddMessage("name", "message 2", 0, 0);

            nextScriptStep.Set();

            scriptTask.Wait(100).Should().BeTrue();

            message1Received.Should().BeTrue();
            message2Received.Should().BeFalse();
        }

        [TestMethod]
        public void WaitAny_chekcs_only_entries_created_afer_last_check()
        {
            var source = new JournalSource();
            var journal = new SpeechJournal(source, null);
            bool firstCheckExecuted = false;
            bool secondCheckExecuted = false;

            source.AddMessage("name", "message1", new ObjectId(0), 0);
            journal.When("message1", () => { firstCheckExecuted = true; }).WhenTimeout(() => { }).WaitAny(TimeSpan.FromMilliseconds(1));
            journal.When("message1", () => { secondCheckExecuted = true; }).WhenTimeout(() => { }).WaitAny(TimeSpan.FromMilliseconds(1));

            firstCheckExecuted.Should().BeTrue();
            secondCheckExecuted.Should().BeFalse();
        }

        [TestMethod]
        public void Can_enumerate_and_add_to_journal_concurrently()
        {
            var source = new JournalSource();
            var journal = new SpeechJournal(source, null);

            source.AddMessage("name", "first message", new ObjectId(0), 0);

            using (var enumerator = journal.GetEnumerator())
            {
                enumerator.MoveNext().Should().BeTrue();
                enumerator.Current.Message.Should().Be("first message");
                source.AddMessage("name", "second, concurrently added message", new ObjectId(0), 0);
                enumerator.MoveNext().Should().BeFalse();
            }
        }

        [TestMethod]
        public void Can_see_entry_received_after_journal_deletion()
        {
            var source = new JournalSource();
            var journal = new SpeechJournal(source, null);

            source.AddMessage("name", "first message", new ObjectId(0), 0);
            journal.Delete();
            source.AddMessage("name", "message after delete", new ObjectId(0), 0);

            journal.Contains("message after delete").Should().BeTrue();
        }

        [TestMethod]
        public void Contains_cannot_see_entry_received_before_journal_deletion()
        {
            var source = new JournalSource();
            var journal = new SpeechJournal(source, null);

            source.AddMessage("name", "message before delete", new ObjectId(0), 0);
            journal.Delete();

            journal.Contains("message before delete").Should().BeFalse();
        }

        [TestMethod]
        public void WaitAny_cannot_see_entry_received_before_journal_deletion()
        {
            var source = new JournalSource();
            var journal = new SpeechJournal(source, null);

            source.AddMessage("name", "message before delete", new ObjectId(0), 0);
            journal.Delete();

            bool canSeeMessageBeforeDelete = false;
            journal.When("message before delete", e => canSeeMessageBeforeDelete = true)
                .WhenTimeout(() => { })
                .WaitAny(TimeSpan.FromMilliseconds(1));

            canSeeMessageBeforeDelete.Should().BeFalse();
        }

        [TestMethod]
        public void Cannot_see_entries_received_before_journal_instantiation()
        {
            var source = new JournalSource();
            source.AddMessage("name", "message before instantiation", new ObjectId(0), 0);

            var journal = new SpeechJournal(source, null);

            journal.Contains("message before instantiation").Should().BeFalse();
        }

        [TestMethod]
        public void Can_see_entries_received_after_journal_instantiation()
        {
            var source = new JournalSource();
            var journal = new SpeechJournal(source, null);

            source.AddMessage("name", "message after instantiation", new ObjectId(0), 0);

            journal.Contains("message after instantiation").Should().BeTrue();
        }

        [TestMethod]
        public void ContainsAnyWord_is_not_affected_by_previous_call_to_WaitAny()
        {
            var source = new JournalSource();
            var journal = new SpeechJournal(source, null);

            source.AddMessage("name", "message1", new ObjectId(0), 0);

            journal.When("message1", () => { }).WaitAny(TimeSpan.FromMilliseconds(1));

            journal.ContainsAnyWord("message1").Any().Should().BeTrue();
        }

    }
}
