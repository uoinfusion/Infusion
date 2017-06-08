using System;
using System.Collections.Generic;
using Infusion.Packets;
using Infusion.Packets.Client;

namespace Infusion.Gumps
{
    internal sealed class TriggerGumpResponse : GumpResponse
    {
        private readonly uint selectedTriggerId;
        private readonly Action<GumpMenuSelectionRequest> triggerGump;
        private readonly uint[] selectedCheckBoxIds;
        private readonly Tuple<ushort, string>[] textEntries;

        public TriggerGumpResponse(Gump gump, uint selectedTriggerId, Action<GumpMenuSelectionRequest> triggerGump, uint[] selectedCheckBoxIds = null, Tuple<ushort, string>[] textEntries = null) : base(gump)
        {
            this.textEntries = textEntries ?? new Tuple<ushort, string>[] {};
            this.selectedTriggerId = selectedTriggerId;
            this.triggerGump = triggerGump;
            this.selectedCheckBoxIds = selectedCheckBoxIds ?? new uint[] {};
        }

        public override void Execute()
        {
            triggerGump(new GumpMenuSelectionRequest(Gump.Id, Gump.GumpId, selectedTriggerId, selectedCheckBoxIds, textEntries));

        }
    }
}