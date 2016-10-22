using UltimaRX.Packets;

namespace UltimaRX.IO
{
    public class DiagnosticPushStream : IDiagnosticPushStream
    {
        private readonly DiagnosticPacketFormatter formatter;

        public DiagnosticPushStream() : this(string.Empty)
        {
        }

        public DiagnosticPushStream(string header)
        {
            formatter = new DiagnosticPacketFormatter(header);
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

        public IPushStream BaseStream { get; set; }

        public void FinishPacket(Packet packet)
        {
            formatter.FinishPacket(packet);
            OnPacketFinished(packet);
        }

        public string Flush()
        {
            return formatter.Flush();
        }

        protected virtual void OnPacketFinished(Packet packet)
        {
        }
    }
}