using UltimaRX.Packets;

namespace UltimaRX.IO
{
    public class NullDiagnosticPushStream : IDiagnosticPushStream
    {
        public static NullDiagnosticPushStream Instance { get; } = new NullDiagnosticPushStream();

        public void Dispose()
        {
            BaseStream.Dispose();
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            BaseStream.Write(buffer, offset, count);
        }

        public IPushStream BaseStream { get; set; }

        public void FinishPacket(Packet packet)
        {
        }
    }
}