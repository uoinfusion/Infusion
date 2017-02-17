using System;
using System.Threading;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltimaRX.Proxy.InjectionApi;

namespace UltimaRX.Proxy.Tests.InjectionApi
{
    [TestClass]
    public class CommandHandlerTests
    {
        private CommandHandler scriptHandler;

        [TestInitialize]
        public void Initialize()
        {
            scriptHandler = new CommandHandler();
        }

        [TestMethod]
        public void Can_create_two_scripts_with_different_names()
        {
            scriptHandler.RegisterCommand("testName1", () => { });
            scriptHandler.RegisterCommand("testName2", () => { });
        }

        [TestMethod]
        public void Can_invoke_script_parameterless_script()
        {
            var ev = new AutoResetEvent(false);
            scriptHandler.RegisterCommand("testName", () => ev.Set());

            scriptHandler.Invoke(",testName");
            ev.WaitOne(1000).Should().BeTrue();
        }

        [TestMethod]
        public void Can_create_parametrized_script()
        {
            scriptHandler.RegisterCommand("testName", parameters => { });
        }

        [TestMethod]
        public void Can_invoke_parametrized_script()
        {
            var actualParameters = string.Empty;
            var ev = new AutoResetEvent(false);

            scriptHandler.RegisterCommand("testName", parameters =>
            {
                actualParameters = parameters;
                ev.Set();
            });

            scriptHandler.Invoke(",testName parameter1 parameter2");
            ev.WaitOne(1000).Should().BeTrue();
            actualParameters.Should().Be("parameter1 parameter2");
        }

        [TestMethod]
        public void Can_return_registered_script_names()
        {
            scriptHandler.RegisterCommand("testName", () => { });
            scriptHandler.RegisterCommand("testName2", parameters => { });

            scriptHandler.CommandNames.Should().Contain("testName").And.Contain("testName2");
        }

        [TestMethod]
        public void Can_recognize_script_invocation_syntax()
        {
            scriptHandler.IsInvocationSyntax(",testName").Should().BeTrue();
            scriptHandler.IsInvocationSyntax("testName").Should().BeFalse();
        }

        [TestMethod]
        public void Can_unregister_parameterless_script()
        {
            scriptHandler.RegisterCommand("testname1", (param) => { });
            scriptHandler.RegisterCommand("testname2", () => { });

            scriptHandler.Unregister("testname2");

            scriptHandler.CommandNames.Should().BeEquivalentTo("testname1");
        }

        [TestMethod]
        public void Can_unregister_parametrized_script()
        {
            scriptHandler.RegisterCommand("testname1", (param) => { });
            scriptHandler.RegisterCommand("testname2", () => { });

            scriptHandler.Unregister("testname1");

            scriptHandler.CommandNames.Should().BeEquivalentTo("testname2");
        }
    }
}
