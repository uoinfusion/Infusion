using System;
using UltimaRX.IO;

namespace UltimaRX.Proxy
{
    public class ConsoleDiagnosticStream : DiagnosticStream
    {
        protected override void OnPacketFinished()
        {
            Console.WriteLine(Flush());
        }
    }
}