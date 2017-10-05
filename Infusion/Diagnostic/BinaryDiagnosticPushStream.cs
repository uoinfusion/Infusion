using System;
using Infusion.IO;
using Infusion.Packets;

namespace Infusion.Diagnostic
{
    internal sealed class BinaryDiagnosticPushStream
    {
        private readonly object flushLock = new object(); 
        private readonly IPushStream diagnosticOutputStream;
        private readonly BinaryPushStreamWriter writer;

        public BinaryDiagnosticPushStream(IPushStream diagnosticOutputStream)
        {
            this.diagnosticOutputStream = diagnosticOutputStream;
            this.writer = new BinaryPushStreamWriter(diagnosticOutputStream);
        }

        public void Dispose()
        {
            lock (flushLock)
            {
                BaseStream.Dispose();
                writer.Flush();
                writer.Dispose();
                diagnosticOutputStream.Dispose();
            }
        }

        public IPushStream BaseStream { get; set; }

        public void DumpPacket(Packet packet, DiagnosticStreamDirection direction)
        {
            lock (flushLock)
            {
                writer.Write(DateTime.UtcNow.Ticks);
                writer.Write((uint) direction);
                writer.Write(packet.Payload, 0, packet.Length);
            }
        }

        public void Finish()
        {
            Flush();
        }

        public void Flush()
        {
            lock (flushLock)
            {
                BaseStream?.Flush();
                diagnosticOutputStream.Flush();
            }
        }
    }
}
