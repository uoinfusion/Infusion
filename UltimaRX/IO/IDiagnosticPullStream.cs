using UltimaRX.Packets;

namespace UltimaRX.IO
{
    public interface IDiagnosticPullStream : IPullStream
    {
        IPullStream BaseStream { get; set; }
        void FinishPacket(Packet packet);
    }
}