using System;
using UltimaRX.Packets;
using UltimaRX.Packets.Client;

namespace UltimaRX.Gumps
{
    public sealed class TriggerGumpResponse : GumpResponse
    {
        private readonly Action<Packet> sendPacket;

        public TriggerGumpResponse(Gump gump, uint selectedTriggerId, Action<Packet> sendPacket) : base(gump)
        {
            this.sendPacket = sendPacket;
            SelectedTriggerId = selectedTriggerId;
        }

        public uint SelectedTriggerId { get; }

        public override void Execute()
        {
            sendPacket(CreatePacket());
        }

        public Packet CreatePacket() => new GumpMenuSelectionRequest(Gump.Id, Gump.GumpId, SelectedTriggerId).RawPacket;
    }
}