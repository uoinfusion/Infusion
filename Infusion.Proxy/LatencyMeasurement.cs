using System;

namespace Infusion.Proxy
{
    public class LatencyMeasurement
    {
        public TimeSpan LatencySum { get; private set; }
        public int Count { get; private set; }
        public TimeSpan LatencyMax { get; private set; }
        public TimeSpan LatencyMin { get; private set; } = TimeSpan.MaxValue;

        public TimeSpan LatencyAvg => new TimeSpan(LatencySum.Ticks / Count);

        public void Add(TimeSpan time)
        {
            LatencySum += time;
            Count++;

            if (time > LatencyMax)
                LatencyMax = time;

            if (time < LatencyMin)
                LatencyMin = time;
        }

        public override string ToString() => $"{LatencyMin:fffff};{LatencyAvg:fffff};{LatencyMax:fffff}";
    }
}