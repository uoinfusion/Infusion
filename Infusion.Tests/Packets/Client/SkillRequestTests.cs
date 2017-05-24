using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Infusion.Packets;
using Infusion.Packets.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.Tests.Packets.Client
{
    [TestClass]
    public class SkillRequestTests
    {
        [TestMethod]
        public void Can_serialize_skill_request()
        {
            var packet = new SkillRequest(Skill.Hiding);

            packet.RawPacket.Payload.Should().BeEquivalentTo(new byte[] { 0x12, 0x00, 0x09, 0x24, 0x32, 0x31, 0x20, 0x30, 0x00, });
        }

        [TestMethod]
        public void Can_serialize_spell_request()
        {
            var packet = new SkillRequest(Spell.NightSight);

            packet.RawPacket.Payload.Should().BeEquivalentTo(new byte[] { 0x12, 0x00, 0x06, 0x56, 0x36, 0x00, });
        }
    }
}
