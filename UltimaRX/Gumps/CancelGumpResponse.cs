using System;
using UltimaRX.Packets;
using UltimaRX.Packets.Client;

namespace UltimaRX.Gumps
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