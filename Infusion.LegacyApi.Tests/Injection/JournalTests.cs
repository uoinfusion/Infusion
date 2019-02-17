using FluentAssertions;
using InjectionScript.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Infusion.LegacyApi.Tests.Injection
{
    [TestClass]
    public class JournalTests
    {
        private InjectionProxy injection;

        [TestInitialize]
        public void Initialize() => injection = new InjectionProxy();

        [TestMethod]
        public void InJournal_returns_0_when_empty_journal() => injection.InjectionHost.UO.InJournal("adsf").Should().Be(0);

        [TestMethod]
        public void InJournal_returns_0_when_text_not_found()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));
            injection.ServerApi.Say(injection.Me.PlayerId, "player name", "qwer");
            injection.ServerApi.Say(injection.Me.PlayerId, "player name", "zxcv");

            injection.InjectionHost.UO.InJournal("adsf").Should().Be(0);
        }

        [TestMethod]
        public void Index_of_found_first_entry_is_1()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));
            injection.ServerApi.Say(injection.Me.PlayerId, "player name", "qwer");

            injection.InjectionHost.UO.InJournal("qwer").Should().Be(1);
        }

        [TestMethod]
        public void Index_of_found_entry_grows_as_server_sends_new_speech()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));

            injection.ServerApi.Say(injection.Me.PlayerId, "player name", "qwer");
            injection.InjectionHost.UO.InJournal("qwer").Should().Be(1);

            injection.ServerApi.Say(injection.Me.PlayerId, "player name", "message 2");
            injection.InjectionHost.UO.InJournal("qwer").Should().Be(2);

            injection.ServerApi.Say(injection.Me.PlayerId, "player name", "message 3");
            injection.InjectionHost.UO.InJournal("qwer").Should().Be(3);
        }

        [TestMethod]
        public void InJournal_finds_newer_entry_first()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));

            injection.ServerApi.Say(injection.Me.PlayerId, "player name", "qwer");
            injection.ServerApi.Say(injection.Me.PlayerId, "player name", "qwer");

            injection.InjectionHost.UO.InJournal("qwer").Should().Be(1);
        }

        [TestMethod]
        public void InJournal_first_searches_for_first_string_in_pattern()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));

            injection.ServerApi.Say(injection.Me.PlayerId, "player name", "asdf");
            injection.ServerApi.Say(injection.Me.PlayerId, "player name", "qwer");
            injection.ServerApi.Say(injection.Me.PlayerId, "player name", "zxcv");

            injection.InjectionHost.UO.InJournal("asdf|qwer").Should().Be(3);
            injection.InjectionHost.UO.InJournal("qwer|asdf").Should().Be(2);
        }

        [TestMethod]
        public void InJournal_accepts_patterns_with_cliloc_message_numbers()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));

            injection.ServerApi.Say(injection.Me.PlayerId, "System", "You cannot reach that.");

            injection.InjectionHost.UO.InJournal("cliloc# 0xA258").Should().NotBe(0);
        }

        [TestMethod]
        public void InJournalBetweenTimes_returns_only_entry_created_in_specified_time_span()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));
            injection.TimeSource.StartTime = new System.DateTime(2019, 1, 2, 10, 10, 23, 125);
            injection.TimeSource.Now = injection.TimeSource.StartTime;

            injection.ServerApi.Say(injection.Me.PlayerId, "player name", "asdf");

            injection.TimeSource.Now = new System.DateTime(2019, 1, 2, 10, 10, 25, 0);
            var startTime = injection.InjectionHost.Now();
            injection.ServerApi.Say(injection.Me.PlayerId, "player name", "asdf");

            injection.TimeSource.Now = new System.DateTime(2019, 1, 2, 10, 10, 26, 0);
            var endTime = injection.InjectionHost.Now();

            injection.TimeSource.Now = new System.DateTime(2019, 1, 2, 10, 10, 27, 0);

            injection.ServerApi.Say(injection.Me.PlayerId, "player name", "asdf");

            injection.InjectionHost.UO.InJournalBetweenTimes("asdf", startTime, endTime).Should().Be(2);
        }

        [TestMethod]
        public void InJournalBetweenTimes_returns_0_when_text_outside_time_span()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));
            injection.TimeSource.StartTime = new System.DateTime(2019, 1, 2, 10, 10, 23, 125);
            injection.TimeSource.Now = injection.TimeSource.StartTime;

            injection.ServerApi.Say(injection.Me.PlayerId, "player name", "asdf");

            injection.TimeSource.Now = new System.DateTime(2019, 1, 2, 10, 10, 25, 0);
            var startTime = injection.InjectionHost.Now();

            injection.TimeSource.Now = new System.DateTime(2019, 1, 2, 10, 10, 26, 0);
            var endTime = injection.InjectionHost.Now();

            injection.TimeSource.Now = new System.DateTime(2019, 1, 2, 10, 10, 27, 0);

            injection.ServerApi.Say(injection.Me.PlayerId, "player name", "asdf");

            injection.InjectionHost.UO.InJournalBetweenTimes("asdf", startTime, endTime).Should().Be(0);
        }

        [TestMethod]
        public void InJournalBetweenTimes_returns_0_when_text_inside_time_span_but_outside_limit()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));
            injection.TimeSource.StartTime = new System.DateTime(2019, 1, 2, 10, 10, 23, 125);
            injection.TimeSource.Now = injection.TimeSource.StartTime;

            injection.ServerApi.Say(injection.Me.PlayerId, "player name", "asdf");

            injection.TimeSource.Now = new System.DateTime(2019, 1, 2, 10, 10, 25, 0);
            var startTime = injection.InjectionHost.Now();
            injection.ServerApi.Say(injection.Me.PlayerId, "player name", "asdf");

            injection.TimeSource.Now = new System.DateTime(2019, 1, 2, 10, 10, 26, 0);
            var endTime = injection.InjectionHost.Now();

            injection.TimeSource.Now = new System.DateTime(2019, 1, 2, 10, 10, 27, 0);

            injection.ServerApi.Say(injection.Me.PlayerId, "player name", "asdf");

            injection.InjectionHost.UO.InJournalBetweenTimes("asdf", startTime, endTime, 1).Should().Be(0);
        }

        [TestMethod]
        public void DeleteJournal_clears_journal()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));

            injection.ServerApi.Say(injection.Me.PlayerId, "player name", "qwer");
            injection.InjectionHost.UO.DeleteJournal();

            injection.InjectionHost.UO.InJournal("qwer").Should().Be(0);
        }

        [TestMethod]
        public void DeleteJournal_with_text_removes_journal_entries_containing_text()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));

            injection.ServerApi.Say(injection.Me.PlayerId, "player name", "qwer");
            injection.ServerApi.Say(injection.Me.PlayerId, "player name", "qwer test");
            injection.ServerApi.Say(injection.Me.PlayerId, "player name", "test qwer");
            injection.ServerApi.Say(injection.Me.PlayerId, "player name", "qwer test qwer");
            injection.InjectionHost.UO.DeleteJournal("test");

            injection.InjectionHost.UO.GetJournalText(0).Should().Be("");
            injection.InjectionHost.UO.GetJournalText(1).Should().Be("");
            injection.InjectionHost.UO.GetJournalText(2).Should().Be("");
            injection.InjectionHost.UO.GetJournalText(3).Should().Contain("qwer");
        }

        [TestMethod]
        public void Journal_returns_text_including_name_and_message_at_zero_based_journal_index()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));

            injection.ServerApi.Say(injection.Me.PlayerId, "player name", "asdf");
            injection.ServerApi.Say(injection.Me.PlayerId, "player name", "qwer");
            injection.ServerApi.Say(injection.Me.PlayerId, "player name", "zxcv");

            injection.InjectionHost.UO.GetJournalText(0).Should().Contain("zxcv");
            injection.InjectionHost.UO.GetJournalText(1).Should().Contain("qwer");
            injection.InjectionHost.UO.GetJournalText(2).Should().Contain("asdf");
        }

        [TestMethod]
        public void JournalSerial_returns_serial_at_zero_based_journal_index()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));

            var asdfSerial = injection.Me.PlayerId;
            var qwerSerial = injection.Me.PlayerId + 1;
            var zxcvSerial = injection.Me.PlayerId + 2;

            injection.ServerApi.Say(asdfSerial, "player name", "asdf");
            injection.ServerApi.Say(qwerSerial, "player name", "qwer");
            injection.ServerApi.Say(zxcvSerial, "player name", "zxcv");

            injection.InjectionHost.UO.JournalSerial(0).Should().Be(NumberConversions.ToHex((int)zxcvSerial));
            injection.InjectionHost.UO.JournalSerial(1).Should().Be(NumberConversions.ToHex((int)qwerSerial));
            injection.InjectionHost.UO.JournalSerial(2).Should().Be(NumberConversions.ToHex((int)asdfSerial));
        }


        [TestMethod]
        public void Journal_returns_same_text_at_found_index_decreased_by_1()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));

            injection.ServerApi.Say(injection.Me.PlayerId, "player name", "asdf");
            injection.ServerApi.Say(injection.Me.PlayerId, "player name", "qwer");
            injection.ServerApi.Say(injection.Me.PlayerId, "player name", "zxcv");

            var foundIndex = injection.InjectionHost.UO.InJournal("asdf");
            foundIndex.Should().Be(3);

            injection.InjectionHost.UO.GetJournalText(foundIndex - 1).Should().Contain("asdf");
        }

        [TestMethod]
        public void JournalSerial_returns_same_serial_at_found_index_decreased_by_1()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));

            var asdfSerial = injection.Me.PlayerId;
            var qwerSerial = injection.Me.PlayerId + 1;
            var zxcvSerial = injection.Me.PlayerId + 2;

            injection.ServerApi.Say(asdfSerial, "player name", "asdf");
            injection.ServerApi.Say(qwerSerial, "player name", "qwer");
            injection.ServerApi.Say(zxcvSerial, "player name", "zxcv");

            var foundIndex = injection.InjectionHost.UO.InJournal("asdf");
            foundIndex.Should().Be(3);

            injection.InjectionHost.UO.JournalSerial(foundIndex - 1).Should().Be(NumberConversions.ToHex((int)asdfSerial));
        }

        [TestMethod]
        public void Journal_returns_empty_text_when_no_entry_at_index()
            => injection.InjectionHost.UO.GetJournalText(100).Should().Be(string.Empty);

        [TestMethod]
        public void JournalSerial_returns_0_when_no_entry_at_index()
            => injection.InjectionHost.UO.JournalSerial(100).Should().Be("0x00000000");

        [TestMethod]
        public void SetJournalSerial_clears_text_regardless_text_in_argument()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));

            injection.ServerApi.Say(injection.Me.PlayerId, "player name", "asdf");
            injection.InjectionHost.UO.SetJournalLine(0, "some text");

            injection.InjectionHost.UO.GetJournalText(0).Should().Be(string.Empty);
        }

        [TestMethod]
        public void SetJournalSerial_clears_text()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));

            injection.ServerApi.Say(injection.Me.PlayerId, "player name", "asdf");
            injection.InjectionHost.UO.SetJournalLine(0);

            injection.InjectionHost.UO.GetJournalText(0).Should().Be(string.Empty);
        }

        [TestMethod]
        public void Only_speech_messages_with_body_type_and_of_Speech_type_contains_name()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));

            injection.ServerApi.Say(injection.Me.PlayerId, "player1", "message1", (Color)0, 0x190);
            injection.ServerApi.Say(injection.Me.PlayerId, "player2", "message2", (Color)0, 0);

            injection.InjectionHost.UO.InJournal("player1: message1").Should().NotBe(0);
            injection.InjectionHost.UO.InJournal("player2: message2").Should().Be(0);
        }
    }
}
