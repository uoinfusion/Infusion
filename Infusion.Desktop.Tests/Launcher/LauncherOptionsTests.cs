using System.Net;
using FluentAssertions;
using Infusion.Desktop.Launcher;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.Desktop.Tests.Launcher
{
    [TestClass]
    public class LauncherOptionsTests
    {
        [TestMethod]
        public void Can_resolve_ipaddress_and_port()
        {
            var options = new LauncherOptions() { ServerEndpoint = "127.0.0.1,2593" };

            options.ResolveServerEndpoint().Result.Should().Be(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2593));
        }

        [TestMethod]
        public void Can_resolve_ipaddress_and_port_with_leading_space()
        {
            var options = new LauncherOptions() { ServerEndpoint = "    127.0.0.1,2593" };

            options.ResolveServerEndpoint().Result.Should().Be(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2593));
        }

        [TestMethod]
        public void Can_resolve_ipaddress_and_port_with_trailing_space()
        {
            var options = new LauncherOptions() { ServerEndpoint = "127.0.0.1,2593    " };

            options.ResolveServerEndpoint().Result.Should().Be(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2593));
        }

        [TestMethod]
        public void Can_resolve_ipaddress_and_port_with_additional_space()
        {
            var options = new LauncherOptions() { ServerEndpoint = "127.0.0.1,    2593" };

            options.ResolveServerEndpoint().Result.Should().Be(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2593));
        }

        [TestMethod]
        public void Can_resolve_ipaddress_with_default_port_2593()
        {
            var options = new LauncherOptions() { ServerEndpoint = "127.0.0.1" };

            options.ResolveServerEndpoint().Result.Should().Be(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2593));
        }

        [TestMethod]
        public void Can_resolve_domain_name_and_port()
        {
            var options = new LauncherOptions() { ServerEndpoint = "localhost,2593" };

            options.ResolveServerEndpoint().Result.Should().Be(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2593));
        }

        [TestMethod]
        public void Can_resolve_domain_name_and_port_with_additonal_space()
        {
            var options = new LauncherOptions() { ServerEndpoint = "localhost,2593" };

            options.ResolveServerEndpoint().Result.Should().Be(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2593));
        }

        [TestMethod]
        public void Can_resolve_domain_name_with_default_port_2593()
        {
            var options = new LauncherOptions() { ServerEndpoint = "localhost" };

            options.ResolveServerEndpoint().Result.Should().Be(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2593));
        }

        [TestMethod]
        public void Can_resolve_domain_name_with_default_port_2593_with_leading_space()
        {
            var options = new LauncherOptions() { ServerEndpoint = "     localhost" };

            options.ResolveServerEndpoint().Result.Should().Be(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2593));
        }

        [TestMethod]
        public void Can_resolve_domain_name_with_default_port_2593_with_trailing_space()
        {
            var options = new LauncherOptions() { ServerEndpoint = "localhost     " };

            options.ResolveServerEndpoint().Result.Should().Be(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2593));
        }
    }
}
