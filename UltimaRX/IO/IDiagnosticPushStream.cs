using Infusion.Packets;

namespace Infusion.IO
{
    public interface IDiagnosticPushStream : IPushStream
    {
        IPushStream BaseStream { get; set; }

        void DumpPacket(Packet packet);

        void Finish();
    }
}