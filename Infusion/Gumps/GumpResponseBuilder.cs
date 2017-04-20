using System;
using System.Collections.Generic;
using Infusion.Packets;

namespace Infusion.Gumps
{
    public class GumpResponseBuilder
    {
        private readonly Gump gump;
        private readonly Action<Packet> sendPacket;
        private readonly List<uint> selectedCheckBoxes = new List<uint>();
        private readonly List<Tuple<ushort, string>> textEntries = new List<Tuple<ushort, string>>();

        public GumpResponseBuilder(Gump gump, Action<Packet> sendPacket)
        {
            this.gump = gump;
            this.sendPacket = sendPacket;
        }

        public GumpResponse Trigger(uint triggerId)
        {
            return new TriggerGumpResponse(gump, triggerId, sendPacket, selectedCheckBoxes.ToArray(), textEntries.ToArray());
        }

        public GumpResponse PushButton(string buttonLabel, GumpLabelPosition labelPosition)
        {
            var processor = new SelectControlByLabelGumpParserProcessor(buttonLabel, labelPosition, GumpControls.Button);
            var parser = new GumpParser(processor);
            parser.Parse(gump);

            if (processor.SelectedControldId.HasValue)
            {
                return new TriggerGumpResponse(gump, processor.SelectedControldId.Value, sendPacket, selectedCheckBoxes.ToArray(), textEntries.ToArray());
            }

            return new GumpFailureResponse(gump, $"Cannot find button '{buttonLabel}'.");
        }

        public GumpResponse Cancel()
        {
            return new CancelGumpResponse(gump, sendPacket);
        }

        public GumpResponseBuilder SelectCheckBox(string checkBoxLabel, GumpLabelPosition labelPosition)
        {
            var processor = new SelectControlByLabelGumpParserProcessor(checkBoxLabel, labelPosition, GumpControls.CheckBox);
            var parser = new GumpParser(processor);
            parser.Parse(gump);

            if (processor.SelectedControldId.HasValue)
                selectedCheckBoxes.Add(processor.SelectedControldId.Value);

            return this;
        }

        public GumpResponseBuilder SetTextEntry(string textEntryLabel, string value, GumpLabelPosition labelPosition)
        {
            var processor = new SelectControlByLabelGumpParserProcessor(textEntryLabel, labelPosition, GumpControls.TextEntry);
            var parser = new GumpParser(processor);
            parser.Parse(gump);

            if (processor.SelectedControldId.HasValue)
                textEntries.Add(new Tuple<ushort, string>((ushort)processor.SelectedControldId.Value, value));

            return this;
        }
    }
}