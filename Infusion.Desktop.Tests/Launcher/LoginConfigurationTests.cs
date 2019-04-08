using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using Infusion.Desktop.Launcher;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.Desktop.Tests.Launcher
{
    [TestClass]
    public class LoginConfigurationTests
    {
        [TestMethod]
        public void Can_insert_infusion_LoginServer_to_empty_file()
        {
            var input = string.Empty;
            var output = new LoginConfiguration(".").SetServerAddress(input, "test.server.com,2593");

            AssertNewLineInsensitive(output, @";Inserted by Infusion
LoginServer=test.server.com,2593");
        }

        [TestMethod]
        public void Can_replace_existing_infusion_LoginServer()
        {
            var input = @";Inserted by Infusion
LoginServer=server.com,2593";
            var output = new LoginConfiguration(".").SetServerAddress(input, "new.server.com,33333");

            AssertNewLineInsensitive(output, @";Inserted by Infusion
LoginServer=new.server.com,33333");
        }

        [TestMethod]
        public void Can_work_with_unix_eol()
        {
            var input = ";Inserted by Infusion\nLoginServer=server.com,2593";
            var output = new LoginConfiguration(".").SetServerAddress(input, "new.server.com,33333");

            AssertNewLineInsensitive(output, @";Inserted by Infusion
LoginServer=new.server.com,33333");

        }

        [TestMethod]
        public void Can_replace_existing_infusion_LoginServer_When_file_with_any_comments()
        {
            var input = @"; Some existing comment

;Inserted by Infusion
LoginServer=server.com,2593

; Some existing comment";
            var output = new LoginConfiguration(".").SetServerAddress(input, "new.server.com,33333");

            var expected = @"; Some existing comment

;Inserted by Infusion
LoginServer=new.server.com,33333

; Some existing comment";

            AssertNewLineInsensitive(output, expected);
        }

        [TestMethod]
        public void Can_insert_infusion_LoginServer_When_file_with_any_comments()
        {
            var input = @"; Some existing comment

; Some existing comment";
            var output = new LoginConfiguration(".").SetServerAddress(input, "new.server.com,33333");

            var expected = @"; Some existing comment

; Some existing comment
;Inserted by Infusion
LoginServer=new.server.com,33333";

            AssertNewLineInsensitive(output, expected);
        }

        [TestMethod]
        public void Can_comment_LoginServer_When_inserting_infusion_LoginServer()
        {
            var input = @"LoginServer=non.infusion.login.server.com,2593";
            var output = new LoginConfiguration(".").SetServerAddress(input, "new.server.com,33333");

            var expected = @";LoginServer=non.infusion.login.server.com,2593
;Inserted by Infusion
LoginServer=new.server.com,33333";

            AssertNewLineInsensitive(output, expected);
        }

        [TestMethod]
        public void Can_comment_LoginServer_When_replacing_infusion_LoginServer()
        {
            var input = @"
LoginServer=login.server.to.be.commented.out.com,2593
;Inserted by Infusion
LoginServer=old.com,2593
LoginServer=login.server.to.be.commented.out.com,2593";

            var expected = @"
;LoginServer=login.server.to.be.commented.out.com,2593
;Inserted by Infusion
LoginServer=new.server.com,33333
;LoginServer=login.server.to.be.commented.out.com,2593";

            var output = new LoginConfiguration(".").SetServerAddress(input, "new.server.com,33333");

            AssertNewLineInsensitive(output, expected);
        }

        [TestMethod]
        public void Can_insert_infusion_LoginServer_When_fusion_comment_without_LoginServer_on_next_line()
        {
            var input = @"
;Inserted by Infusion
;something other than LoginServer";

            var expected = @"
;Inserted by Infusion
LoginServer=new.server.com,33333";

            var output = new LoginConfiguration(".").SetServerAddress(input, "new.server.com,33333");

            AssertNewLineInsensitive(output, expected);
        }

        private void AssertNewLineInsensitive(string actual, string expected)
        {
            actual = Regex.Replace(actual, @"\r\n?|\n", Environment.NewLine);
            expected = Regex.Replace(expected, @"\r\n?|\n", Environment.NewLine);

            actual.Should().Be(expected);
        }
    }
}
