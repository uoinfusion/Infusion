using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Infusion.LegacyApi.Tests
{
    [TestClass]
    public class CorpsesTests
    {
        [TestMethod]
        public void Amount_is_corpse_type()
        {
            var payload = new byte[]
            {
                0x1A, 0x00, 0x11, 0xC0, 0x0B, 0x7E, 0x06,
                0x20, 0x06, // corpse type
                0x00, 0x05, // amount, has to be interpreted as corpse type
                0x8A, 0x3B, 0x0C, 0x20, 0x03, 0x01,
            };

            var testProxy = new InfusionTestProxy();
            testProxy.PacketReceivedFromServer(payload);

            var corpse = testProxy.Api.Corpses.Single();
            corpse.CorpseType.Should().Be((ModelId)0x0005);
        }

        [TestMethod]
        public void Keeps_corpse_type_when_server_sends_corpse_name()
        {
            var testProxy = new InfusionTestProxy();

            var corpseObjectInfo = new byte[]
            {
                0x1A, 0x00, 0x11, 0xC0, 0x09, 0x8F, 0xD6, 0x20, 0x06, 0x00, 0xED, 0x8A, 0x44,
                0x0C, 0xDD, 0x07, 0x00,
            };
            testProxy.PacketReceivedFromServer(corpseObjectInfo);


            var corpseNameSpeech = new byte[]
            {
                0x1C, 0x00, 0x3A, 0x40, 0x09, 0x8F, 0xD6, 0x20, 0x06, 0x06, 0x03, 0xB2, 0x00, 0x03, 0x42, 0x6F,
                0x64, 0x79, 0x20, 0x6F, 0x66, 0x20, 0x53, 0x72, 0x6E, 0x65, 0x63, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x42, 0x6F, 0x64, 0x79,
                0x20, 0x6F, 0x66, 0x20, 0x53, 0x72, 0x6E, 0x65, 0x63, 0x00,
            };

            testProxy.PacketReceivedFromServer(corpseNameSpeech);

            var corpse = testProxy.Api.Corpses.Single();
            corpse.Name.Should().Be("Body of Srnec");
            corpse.CorpseType.Should().Be((ModelId)0x00ED);
        }
    }
}
