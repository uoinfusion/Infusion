using Infusion.IO;
using Infusion.Packets;

namespace Infusion.Diagnostic
{
    public class TextDiagnosticPullStream : IDiagnosticPullStream
    {
        private readonly DiagnosticPacketFormatter formatter;

        public TextDiagnosticPullStream() : this(string.Empty)
        {
        }

        public TextDiagnosticPullStream(string header)
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
            formatter.DumpPacket(packet);
            OnPacketFinished(packet);
        }

        public void StartPacket()
        {
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