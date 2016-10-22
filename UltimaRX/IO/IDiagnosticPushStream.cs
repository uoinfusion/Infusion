using UltimaRX.Packets;

namespace UltimaRX.IO
{
    public interface IDiagnosticPushStream : IPushStream
    {
        IPushStream BaseStream { get; set; }

        void DumpPacket(Packet packet);

        void Finish();
    }
}