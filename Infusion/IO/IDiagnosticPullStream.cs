using Infusion.Packets;

namespace Infusion.IO
{
    public interface IDiagnosticPullStream : IPullStream
    {
        IPullStream BaseStream { get; set; }
        void FinishPacket(Packet packet);
    }
}