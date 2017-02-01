using System;
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
        public void Throws_when_creating_script_with_already_existing_name()
        {
            scriptHandler.RegisterCommand("testName1", () => { });

            Action action = () => scriptHandler.RegisterCommand("testName1", () => { });
            action.ShouldThrow<CommandInvocationException>();
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
            var executed = false;
            scriptHandler.RegisterCommand("testName", () => executed = true);

            scriptHandler.Invoke(",testName");
            executed.Should().BeTrue();
        }

        [TestMethod]
        public void Throws_when_invoking_unknown_parameterless_script()
        {
            Action action = () => scriptHandler.Invoke(",testName");

            action.ShouldThrow<CommandInvocationException>();
        }

        [TestMethod]
        public void Can_create_parametrized_script()
        {
            scriptHandler.RegisterCommand("testName", parameters => { });
        }

        [TestMethod]
        public void Throws_when_parameterless_and_parametrized_scripts_are_created_with_same_name()
        {
            scriptHandler.RegisterCommand("testName", () => { });
            Action action = () => scriptHandler.RegisterCommand("testName", parameters => { });

            action.ShouldThrow<CommandInvocationException>();
        }

        [TestMethod]
        public void Throws_when_parametrized_and_parameterless_scripts_are_created_with_same_name()
        {
            scriptHandler.RegisterCommand("testName", parameters => { });
            Action action = () => scriptHandler.RegisterCommand("testName", () => { });

            action.ShouldThrow<CommandInvocationException>();
        }

        [TestMethod]
        public void Can_invoke_parametrized_script()
        {
            var actualParameters = string.Empty;

            scriptHandler.RegisterCommand("testName", parameters => { actualParameters = parameters; });

            scriptHandler.Invoke(",testName parameter1 parameter2");
            actualParameters.Should().Be("parameter1 parameter2");
        }

        [TestMethod]
        public void Throws_when_no_parameters_for_parametrized_script()
        {
            scriptHandler.RegisterCommand("testName", parameters => { });

            Action action = () => scriptHandler.Invoke(",testName ");
            action.ShouldThrow<CommandInvocationException>();
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
