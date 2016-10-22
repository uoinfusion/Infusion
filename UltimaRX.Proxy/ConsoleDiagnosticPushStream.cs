using System;
using UltimaRX.IO;
using UltimaRX.Packets;

namespace UltimaRX.Proxy
{
    public class ConsoleDiagnosticPushStream : DiagnosticPushStream
    {
        public ConsoleDiagnosticPushStream(string header) : base(header)
        {
        }

        protected override void OnPacketFinished(Packet packet)
        {
            Console.WriteLine(Flush());
        }
    }
}