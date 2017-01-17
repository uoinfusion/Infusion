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
    public class GumpResponseBuilderTests
    {
        [TestMethod]
        public void Can_create_response_for_button_in_front_of_requested_label()
        {
            var gump = new Gump(1, 2, "{Text 50 215 955 0}{Button 13 215 4005 4007 1 0 9}", new [] { "test label" });
            var response = new GumpResponseBuilder(gump, packet => { }).PushButton("test label");

            response.Should().BeOfType<TriggerGumpResponse>().Which.SelectedTriggerId.Should().Be(9);
        }

        [TestMethod]
        public void Can_create_failure_response_for_non_existent_label()
        {
            var gump = new Gump(1, 2, "{Text 50 215 955 0}{Button 13 215 4005 4007 1 0 9}", new[] { "test label" });
            var response = new GumpResponseBuilder(gump, packet => { }).PushButton("non existent label");

            response.Should()
                .BeOfType<GumpFailureResponse>()
                .Which.FailureMessage.Should()
                .Be("Cannot find button 'non existent label'.");
        }

        [TestMethod]
        public void Can_create_cancel_response()
        {
            var gump = new Gump(1, 2, "{Text 50 215 955 0}{Button 13 215 4005 4007 1 0 9}", new[] { "test label" });
            var response = new GumpResponseBuilder(gump, packet => { }).Cancel();

            response.Should().BeOfType<CancelGumpResponse>();
        }
    }
}
