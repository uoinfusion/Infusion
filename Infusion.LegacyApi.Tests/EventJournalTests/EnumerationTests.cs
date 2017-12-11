using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Infusion.LegacyApi.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.LegacyApi.Tests.EventJournalTests
{
    [TestClass]
    public class EnumerationTests
    {
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

    }
}
