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
    }
}