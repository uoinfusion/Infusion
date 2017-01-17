using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltimaRX.Gumps;

namespace UltimaRX.Tests.Gumps
{
    [TestClass]
    public class TriggerGumpResponseTests
    {
        [TestMethod]
        public void Can_create_packet_for_selected_trigger()
        {
            var gump = new Gump(0x40000DA7, 0x96000495, "{Button 13 215 4005 4007 1 0 9}", new string [] { });
            var response = new TriggerGumpResponse(gump, 9, p => { });
            var rawPacket = response.CreatePacket();

            rawPacket.Payload.Should().BeEquivalentTo(new byte[]
            {
                0xB1, 0x00, 0x17, 0x40, 0x00, 0x0D, 0xA7, 0x96, 0x00, 0x04, 0x95, 0x00, 0x00, 0x00, 0x09, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            });
        }
    }
}
