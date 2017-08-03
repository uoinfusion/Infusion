using System;
using Infusion.IO;
using Infusion.Packets;

namespace Infusion.Diagnostic
{
    internal sealed class InfusionBinaryDiagnosticPushStream : IDiagnosticPushStream
    {
        private readonly DiagnosticStreamDirection direction;
        private readonly Func<BinaryDiagnosticPushStream> pushStreamProvider;

        public InfusionBinaryDiagnosticPushStream(DiagnosticStreamDirection direction, Func<BinaryDiagnosticPushStream> pushStreamProvider)
        {
            this.direction = direction;
            this.pushStreamProvider = pushStreamProvider;
        }

        public void Dispose()
        {
            BaseStream?.Dispose();
            BaseStream = null;
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            BaseStream.Write(buffer, offset, count);
        }

        public void WriteByte(byte value)
        {
            BaseStream.WriteByte(value);
        }

        public void Flush()
        {
            BaseStream?.Flush();
            pushStreamProvider().Flush();
        }

        public IPushStream BaseStream { get; set; }

        public void DumpPacket(Packet packet)
        {
            pushStreamProvider()?.DumpPacket(packet, direction);
        }

        public void Finish()
        {
            pushStreamProvider()?.Finish();
        }
    }
}
