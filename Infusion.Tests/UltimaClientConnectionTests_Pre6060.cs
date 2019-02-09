using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Infusion.Tests
{
    [TestClass]
    public class UltimaClientConnectionTests_Pre6060
    {
        [TestMethod]
        public void
    Given_connection_in_Initial_status_When_receives_login_seed_Then_connection_enters_ServerLogin_status()
        {
            var inputStream = new TestPullStream(new List<byte[]> { FakePackets.InitialLoginSeed_Pre6060 });

            var connection = new UltimaClientConnection(UltimaClientConnectionStatus.Initial);
            connection.ReceiveBatch(inputStream, inputStream.Length);

            connection.Status.Should().Be(UltimaClientConnectionStatus.AfterInitialSeed);
        }
    }
}
