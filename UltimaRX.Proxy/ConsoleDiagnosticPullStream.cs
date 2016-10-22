using System;
using UltimaRX.IO;
using UltimaRX.Packets;

namespace UltimaRX.Proxy
{
    // TODO: do not have special type for each logging backend, inject backend instead
    public class ConsoleDiagnosticPullStream : DiagnosticPullStream
    {
        public ConsoleDiagnosticPullStream(string header) : base(header)
        {
        }

        protected override void OnPacketFinished(Packet packet)
        {
            Console.WriteLine(Flush());
        }
    }
}