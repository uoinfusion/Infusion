using System;
using Infusion.Packets;
using Infusion.Packets.Client;

namespace Infusion.Gumps
{
    internal sealed class CancelGumpResponse : GumpResponse
    {
        private readonly Action<GumpMenuSelectionRequest> triggerGump;

        public CancelGumpResponse(Gump gump, Action<GumpMenuSelectionRequest> triggerGump) : base(gump)
        {
            this.triggerGump = triggerGump;
        }

        public override void Execute()
        {
            triggerGump(new GumpMenuSelectionRequest(Gump.Id, Gump.GumpId, new GumpControlId(0), new GumpControlId[] { }, new Tuple<ushort, string>[] { }));
        }
    }
}