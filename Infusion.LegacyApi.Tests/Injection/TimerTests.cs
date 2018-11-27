using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Tests.Injection
{
    [TestClass]
    public class TimerTests
    {
        private InjectionProxy injection;

        [TestInitialize]
        public void Initialize()
        {
            injection = new InjectionProxy();
        }

        [TestMethod]
        public void Timer_value_equals_to_number_of_tenths_of_seconds()
        {
            var start = injection.InjectionHost.UO.Timer();
            injection.InjectionHost.Wait(100);
            var end = injection.InjectionHost.UO.Timer();

            int duration = end - start;

            duration.Should().BeGreaterOrEqualTo(1);
            duration.Should().BeLessThan(10);
        }
    }
}
