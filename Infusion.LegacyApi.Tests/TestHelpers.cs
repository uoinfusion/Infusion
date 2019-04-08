using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Tests
{
    public static class TestHelpers
    {
        public static void AssertWaitFastSuccess(this Task task)
            => task.Wait(FastTimeout).Should().BeTrue($"Unexpected Wait timeout {FastTimeout}");
        public static void AssertWaitSlowSuccess(this Task task)
            => task.Wait(SlowTimeout).Should().BeTrue($"Unexpected Wait timeout {SlowTimeout}");

        public static void AssertWaitOneSuccess(this EventWaitHandle ev)
            => ev.WaitOne(FastTimeout).Should().BeTrue($"Unexpected WaitOne timeout {FastTimeout}");

        public static void WaitOneFast(this EventWaitHandle ev) => ev.WaitOne(FastTimeout);

#if SLOW_MACHINE
        public static TimeSpan FastTimeout { get; } = TimeSpan.FromMilliseconds(2000);
#else
        public static TimeSpan FastTimeout { get; } = TimeSpan.FromMilliseconds(100);
#endif

        #if SLOW_MACHINE
        public static TimeSpan SlowTimeout { get; } = TimeSpan.FromMilliseconds(10000);
#else
        public static TimeSpan SlowTimeout { get; } = TimeSpan.FromMilliseconds(1000);
#endif
    }
}
