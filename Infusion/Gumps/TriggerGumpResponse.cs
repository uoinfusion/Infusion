using System;
using System.Collections.Generic;
using Infusion.Packets;
using Infusion.Packets.Client;

namespace Infusion.Gumps
{
    internal sealed class TriggerGumpResponse : GumpResponse
    {
        private readonly uint selectedTriggerId;
        private readonly uint[] selectedCheckBoxIds;
        private readonly Tuple<ushort, string>[] textEntries;
        private readonly Action<Packet> sendPacket;

        public TriggerGumpResponse(Gump gump, uint selectedTriggerId, Action<Packet> sendPacket, uint[] selectedCheckBoxIds = null, Tuple<ushort, string>[] textEntries = null) : base(gump)
        {
            this.sendPacket = sendPacket;
            this.textEntries = textEntries ?? new Tuple<ushort, string>[] {};
            this.selectedTriggerId = selectedTriggerId;
            this.selectedCheckBoxIds = selectedCheckBoxIds ?? new uint[] {};
        }

        public override void Execute()
        {
            sendPacket(new GumpMenuSelectionRequest(Gump.Id, Gump.GumpId, selectedTriggerId, selectedCheckBoxIds, textEntries).RawPacket);
        }
    }
}