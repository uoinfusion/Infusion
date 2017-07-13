using Infusion.IO;
using Infusion.Packets;

namespace Infusion.Diagnostic
{
    public sealed class NullDiagnosticPushStream : IDiagnosticPushStream
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

        public void WriteByte(byte value)
        {
            BaseStream.WriteByte(value);
        }

        public void Flush()
        {
            BaseStream.Flush();
        }

        public IPushStream BaseStream { get; set; }

        public void DumpPacket(Packet packet)
        {
        }

        public void Finish()
        {
        }
    }
}