using System;
using Infusion.Packets;
using Infusion.Packets.Client;

namespace Infusion.Gumps
{
    internal sealed class TriggerGumpResponse : GumpResponse
    {
        private readonly GumpControlId[] selectedCheckBoxIds;
        private readonly GumpControlId selectedTriggerId;
        private readonly Tuple<ushort, string>[] textEntries;
        private readonly Action<GumpMenuSelectionRequest> triggerGump;

        public TriggerGumpResponse(Gump gump, GumpControlId selectedTriggerId,
            Action<GumpMenuSelectionRequest> triggerGump, GumpControlId[] selectedCheckBoxIds = null,
            Tuple<ushort, string>[] textEntries = null) : base(gump)
        {
            this.textEntries = textEntries ?? new Tuple<ushort, string>[] { };
            this.selectedTriggerId = selectedTriggerId;
            this.triggerGump = triggerGump;
            this.selectedCheckBoxIds = selectedCheckBoxIds ?? Array.Empty<GumpControlId>();
        }

        public override void Execute()
        {
            triggerGump(new GumpMenuSelectionRequest(Gump.Id, Gump.GumpId, selectedTriggerId, selectedCheckBoxIds,
                textEntries));
        }
    }
}