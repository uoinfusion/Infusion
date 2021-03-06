﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Tests
{
    [TestClass]
    public class ClilocTests
    {
        [TestMethod]
        public void Can_translate_cliloc_message_with_arguments()
        {
            var proxy = new InfusionTestProxy();
            proxy.ClilocSource.Add(0x001005BD, "~1_PREFIX~~2_NAME~~3_SUFFIX~");

            proxy.PacketReceivedFromServer(new byte[]
            {
                0xC1, 0x00, 0x60, 0x02, 0x70, 0x72, 0x62, 0x01, 0x90, 0x06, 0x00, 0x35, 0x00, 0x03, 0x00, 0x10,
                0x05, 0xBD, 0x4B, 0x75, 0x72, 0x74, 0x20, 0x50, 0x6F, 0x6E, 0x64, 0x20, 0x74, 0x68, 0x65, 0x20,
                0x4D, 0x69, 0x6E, 0x65, 0x72, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x20, 0x00, 0x09, 0x00, 0x4B, 0x00, 0x75, 0x00, 0x72, 0x00, 0x74, 0x00, 0x20, 0x00, 0x50, 0x00,
                0x6F, 0x00, 0x6E, 0x00, 0x64, 0x00, 0x20, 0x00, 0x74, 0x00, 0x68, 0x00, 0x65, 0x00, 0x20, 0x00,
                0x4D, 0x00, 0x69, 0x00, 0x6E, 0x00, 0x65, 0x00, 0x72, 0x00, 0x09, 0x00, 0x20, 0x00, 0x00, 0x00,
            });

            var message = proxy.Api.Journal.First().Message;
            message.Should().Be(" Kurt Pond the Miner ");
        }
    }
}
