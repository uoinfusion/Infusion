using System;
using UltimaRX.Packets;
using UltimaRX.Packets.Client;

namespace UltimaRX.Gumps
{
    internal sealed class TriggerGumpResponse : GumpResponse
    {
        private readonly uint selectedTriggerId;
        private readonly Action<Packet> sendPacket;

        public TriggerGumpResponse(Gump gump, uint selectedTriggerId, Action<Packet> sendPacket) : base(gump)
        {
            this.sendPacket = sendPacket;
            this.selectedTriggerId = selectedTriggerId;
        }

        public override void Execute()
        {
            sendPacket(new GumpMenuSelectionRequest(Gump.Id, Gump.GumpId, selectedTriggerId).RawPacket);
        }
    }
}