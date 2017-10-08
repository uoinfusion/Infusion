using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Infusion.Packets;

namespace Infusion.Proxy
{
    public class LatencyMeter
    {
        private readonly object measurementLock = new object();
        public LatencyMeasurement OverallMeasurement { get; } = new LatencyMeasurement();

        public Dictionary<int, LatencyMeasurement> PerPacketMeasurement { get; } =
            new Dictionary<int, LatencyMeasurement>();

        public void Measure(Packet packet, Action measuredAction)
        {
            var watch = Stopwatch.StartNew();

            try
            {
                measuredAction();
            }
            finally
            {
                watch.Stop();

                lock (measurementLock)
                {
                    OverallMeasurement.Add(watch.Elapsed);
                    AddPacketMeasurement(packet, watch.Elapsed);
                }
            }
        }

        private void AddPacketMeasurement(Packet packet, TimeSpan measuredTime)
        {
            if (!PerPacketMeasurement.TryGetValue(packet.Id, out var measurement))
            {
                measurement = new LatencyMeasurement();
                PerPacketMeasurement.Add(packet.Id, measurement);
            }

            measurement.Add(measuredTime);
        }

        public override string ToString()
        {
            return $"Overall: {OverallMeasurement}" + Environment.NewLine +
                   PerPacketMeasurement
                       .OrderByDescending(x => x.Value.LatencyMax)
                       .Select(x => $"{x.Key:X2}: {x.Value}")
                       .Aggregate((l, r) => l + Environment.NewLine + r);
        }
    }
}