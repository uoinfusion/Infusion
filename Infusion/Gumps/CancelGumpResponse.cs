using System;
using Infusion.Packets;
using Infusion.Packets.Client;

namespace Infusion.Gumps
{
    internal sealed class CancelGumpResponse : GumpResponse
    {
        private readonly Gump gump;
        private readonly Action<Packet> sendPacket;

        public CancelGumpResponse(Gump gump, Action<Packet> sendPacket) : base(gump)
        {
            this.gump = gump;
            this.sendPacket = sendPacket;
        }

        public override void Execute()
        {
            sendPacket(new GumpMenuSelectionRequest(Gump.Id, Gump.GumpId, 0).RawPacket);
        }
    }
}