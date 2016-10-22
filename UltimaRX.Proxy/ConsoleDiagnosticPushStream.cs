using System;
using UltimaRX.IO;
using UltimaRX.Packets;

namespace UltimaRX.Proxy
{
    // TODO: do not have special type for each logging backend, inject backend to DiagnosticPushStream instead
    public class ConsoleDiagnosticPushStream : DiagnosticPushStream
    {
        public ConsoleDiagnosticPushStream(string header) : base(header)
        {
        }

        protected override void OnPacketFinished()
        {
            Console.WriteLine(Flush());
        }
    }
}