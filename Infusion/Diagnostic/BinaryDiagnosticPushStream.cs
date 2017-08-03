using System;
using Infusion.IO;
using Infusion.Packets;

namespace Infusion.Diagnostic
{
    internal sealed class BinaryDiagnosticPushStream
    {
        private readonly IPushStream diagnosticOutputStream;
        private readonly BinaryPushStreamWriter writer;

        public BinaryDiagnosticPushStream(IPushStream diagnosticOutputStream)
        {
            this.diagnosticOutputStream = diagnosticOutputStream;
            this.writer = new BinaryPushStreamWriter(diagnosticOutputStream);
        }

        public void Dispose()
        {
            BaseStream.Dispose();
            writer.Flush();
            writer.Dispose();
            diagnosticOutputStream.Dispose();
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            BaseStream.Write(buffer, offset, count);
        }

        public void WriteByte(byte value)
        {
            BaseStream.WriteByte(value);
        }

        public IPushStream BaseStream { get; set; }

        public void DumpPacket(Packet packet, DiagnosticStreamDirection direction)
        {
            writer.Write(DateTime.UtcNow.Ticks);
            writer.Write((uint)direction);
            writer.Write(packet.Payload, 0, packet.Length);
        }

        public void Finish()
        {
            Flush();
        }

        public void Flush()
        {
            BaseStream?.Flush();
            diagnosticOutputStream.Flush();
        }
    }
}
