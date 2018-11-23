using FluentAssertions;
using Infusion.LegacyApi.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Tests
{
    [TestClass]
    public class SkillTests
    {
        [TestMethod]
        public void Single_skill_change_raises_event()
        {
            var testProxy = new InfusionTestProxy();
            var journal = testProxy.Api.CreateEventJournal();
            
            // initialize skill value, SkillChangedEvent requires it
            testProxy.PacketReceivedFromServer(new byte[] { 0x3A, 0x00, 0x0B, 0xFF, 0x00, 0x02, 0x02, 0xC9, 0x02, 0xC9, 0x00, });

            // now it can be changed and SkillChangedEvent can be raised
            testProxy.PacketReceivedFromServer(new byte[] { 0x3A, 0x00, 0x0B, 0xFF, 0x00, 0x02, 0x02, 0xCA, 0x02, 0xCA, 0x00, });

            journal.OfType<SkillChangedEvent>().Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void Single_skill_0_change_raises_event_as_Alchemy()
        {
            var testProxy = new InfusionTestProxy();
            var journal = testProxy.Api.CreateEventJournal();

            testProxy.PacketReceivedFromServer(new byte[] { 0x3A, 0x00, 0x0B, 0xFF, 0x00, 0x00, 0x02, 0xC9, 0x02, 0xC9, 0x00, });
            testProxy.PacketReceivedFromServer(new byte[] { 0x3A, 0x00, 0x0B, 0xFF, 0x00, 0x00, 0x02, 0xCA, 0x02, 0xCA, 0x00, });

            journal.OfType<SkillChangedEvent>().Should().NotBeEmpty();
            journal.OfType<SkillChangedEvent>().First().Skill.Should().Be(Skill.Alchemy);
        }
    }
}
