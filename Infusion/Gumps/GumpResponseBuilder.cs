using System;
using Infusion.Packets;

namespace Infusion.Gumps
{
    public class GumpResponseBuilder
    {
        private readonly Gump gump;
        private readonly Action<Packet> sendPacket;

        public GumpResponseBuilder(Gump gump, Action<Packet> sendPacket)
        {
            this.gump = gump;
            this.sendPacket = sendPacket;
        }

        public GumpResponse PushButton(string buttonLabel, GumpLabelPosition labelPosition)
        {
            var processor = new SelectButtonByLabelGumpParserProcessor(buttonLabel, labelPosition);
            var parser = new GumpParser(processor);
            parser.Parse(gump);

            if (processor.SelectedTriggerId.HasValue)
            {
                return new TriggerGumpResponse(gump, processor.SelectedTriggerId.Value, sendPacket);
            }

            return new GumpFailureResponse(gump, $"Cannot find button '{buttonLabel}'.");
        }

        public GumpResponse Cancel()
        {
            return new CancelGumpResponse(gump, sendPacket);
        }
    }
}