using UltimaRX.Packets;

namespace UltimaRX.IO
{
    public class NullDiagnosticStream : IDiagnosticStream
    {
        public void Dispose()
        {
            BaseStream.Dispose();
        }

        public bool DataAvailable => BaseStream.DataAvailable;

        public int ReadByte()
        {
            return BaseStream.ReadByte();
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return BaseStream.ReadByte();
        }

        public IPullStream BaseStream { get; set; }

        public void FinishPacket(Packet packet)
        {
        }
    }
}