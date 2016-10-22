using UltimaRX.Packets;

namespace UltimaRX.IO
{
    public class DiagnosticPullStream : IDiagnosticPullStream
    {
        private readonly DiagnosticPacketFormatter formatter;

        public DiagnosticPullStream() : this(string.Empty)
        {
        }

        public DiagnosticPullStream(string header)
        {
            formatter = new DiagnosticPacketFormatter(header);
        }

        public void Dispose()
        {
            BaseStream.Dispose();
        }

        bool IPullStream.DataAvailable => BaseStream.DataAvailable;

        public int ReadByte()
        {
            var value = BaseStream.ReadByte();

            if ((value >= byte.MinValue) && (value <= byte.MaxValue))
                formatter.AddByte((byte) value);

            return value;
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            formatter.Header();

            var length = BaseStream.Read(buffer, offset, count);

            if (length > 0)
            {
                for (var i = 0; i < length; i++)
                    formatter.AddByte(buffer[offset + i]);
            }

            return length;
        }

        public IPullStream BaseStream { get; set; }

        public void FinishPacket(Packet packet)
        {
            formatter.FinishPacket(packet);
            OnPacketFinished(packet);
        }

        protected virtual void OnPacketFinished(Packet packet)
        {
        }

        public string Flush()
        {
            return formatter.Flush();
        }
    }
}