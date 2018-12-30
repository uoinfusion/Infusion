using Infusion.IO;
using Infusion.Packets;

namespace Infusion.Diagnostic
{
    internal class TextDiagnosticPushStream : IDiagnosticPushStream
    {
        private readonly DiagnosticPacketFormatter formatter;

        public TextDiagnosticPushStream(PacketDefinitionRegistry packetRegistry) : this(packetRegistry, string.Empty)
        {
        }

        public TextDiagnosticPushStream(PacketDefinitionRegistry packetRegistry, string header)
        {
            formatter = new DiagnosticPacketFormatter(header, packetRegistry);
        }

        public void Dispose()
        {
            BaseStream.Dispose();
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            formatter.Header();

            BaseStream.Write(buffer, offset, count);

            for (var i = 0; i < count; i++)
                formatter.AddByte(buffer[offset + i]);
        }

        public void WriteByte(byte value)
        {
            BaseStream.WriteByte(value);
            formatter.AddByte(value);
        }

        public IPushStream BaseStream { get; set; }

        public void DumpPacket(Packet packet)
        {
            formatter.DumpPacket(packet);
        }

        public void Finish()
        {
            OnPacketFinished();
        }

        public string Flush()
        {
            return formatter.Flush();
        }

        protected virtual void OnPacketFinished()
        {
        }

        void IPushStream.Flush()
        {
        }
    }
}