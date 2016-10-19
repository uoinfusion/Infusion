using UltimaRX.Packets;

namespace UltimaRX.IO
{
    public interface IDiagnosticStream : IPullStream
    {
        IPullStream BaseStream { get; set; }
        void FinishPacket(Packet packet);
    }
}