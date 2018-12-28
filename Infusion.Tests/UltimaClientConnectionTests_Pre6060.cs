using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            connection.ReceiveBatch(inputStream);

            connection.Status.Should().Be(UltimaClientConnectionStatus.ServerLogin);
        }
    }

    [TestClass]
    public class UltimaClientConnectionTests_Post6060
    {
        [TestMethod]
    public void
    Given_connection_in_Initial_status_When_receives_login_seed_Then_connection_enters_ServerLogin_status()
    {
        var inputStream = new TestPullStream(new List<byte[]> { FakePackets.InitialLoginSeed_Post6060 });

        var connection = new UltimaClientConnection(UltimaClientConnectionStatus.Initial);
        connection.ReceiveBatch(inputStream);

        connection.Status.Should().Be(UltimaClientConnectionStatus.ServerLogin);
    }

}
}
