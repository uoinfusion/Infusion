using Infusion.Packets;

namespace Infusion.IO
{
    internal interface IDiagnosticPullStream : IPullStream
    {
        IPullStream BaseStream { get; set; }
        void FinishPacket(Packet packet);
        void StartPacket();
    }
}