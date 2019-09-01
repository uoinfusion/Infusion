using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infusion.LegacyApi.Tests
{
    [TestClass]
    public class SpeechRequestTests
    {
        [TestMethod]
        public void Can_send_one_keyword_speech_request()
        {
            var testProxy = new InfusionTestProxy();

            testProxy.KeywordSource.Add(new Ultima.SpeechEntry(2, "*bank*", 1));
            testProxy.Api.Say("bank");

            testProxy.PacketsSentToServer.Single().Payload.Should().BeEquivalentTo(new byte[] {
                0xAD, 0x00, 0x14, 0xC0, 0x02, 0xB2, 0x00, 0x03, 0x45, 0x4E, 0x55, 0x00, 0x00, 0x10, 0x02, 0x62,
                0x61, 0x6E, 0x6B, 0x00
            });
        }

        [TestMethod]
        public void Can_send_speech_request_without_keyword()
        {
            var testProxy = new InfusionTestProxy();

            testProxy.KeywordSource.Add(new Ultima.SpeechEntry(2, "*bank*", 1));
            testProxy.Api.Say("Cank");

            testProxy.PacketsSentToServer.Single().Payload.Should().BeEquivalentTo(new byte[] {
                0xAD, 0x00, 0x16, 0x00, 0x02, 0xB2, 0x00, 0x03, 0x45, 0x4E, 0x55, 0x00, 0x00, 0x43, 0x00, 0x61,
                0x00, 0x6E, 0x00, 0x6B, 0x00, 0x00,
            });
        }
    }
}
