using System;
using Infusion.Packets;
using Infusion.Packets.Client;

namespace Infusion.Gumps
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