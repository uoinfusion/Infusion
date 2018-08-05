using System;
using System.Collections.Generic;
using Infusion.Packets;
using Infusion.Packets.Client;

namespace Infusion.Gumps
{
    public sealed class GumpResponseBuilder
    {
        private readonly Gump gump;
        private readonly Action<GumpMenuSelectionRequest> triggerGump;
        private readonly List<GumpControlId> selectedCheckBoxes = new List<GumpControlId>();
        private readonly List<Tuple<ushort, string>> textEntries = new List<Tuple<ushort, string>>();

        internal GumpResponseBuilder(Gump gump, Action<GumpMenuSelectionRequest> triggerGump)
        {
            this.gump = gump;
            this.triggerGump = triggerGump;
        }

        public void Trigger(GumpControlId triggerId)
        {
            new TriggerGumpResponse(gump, triggerId, triggerGump, selectedCheckBoxes.ToArray(), textEntries.ToArray()).Execute();
        }

        public void PushButton(string buttonLabel, GumpLabelPosition labelPosition)
        {
            var processor = new SelectControlByLabelGumpParserProcessor(buttonLabel, labelPosition, GumpControls.Button);
            var parser = new GumpParser(processor);
            parser.Parse(gump);

            if (processor.SelectedControldId.HasValue)
            {
                new TriggerGumpResponse(gump, processor.SelectedControldId.Value, triggerGump, selectedCheckBoxes.ToArray(), textEntries.ToArray()).Execute();
                return;
            }

            new GumpFailureResponse(gump, $"Cannot find button '{buttonLabel}'.").Execute();
        }

        public void Cancel()
        {
            new CancelGumpResponse(gump, triggerGump).Execute();
        }

        public GumpResponseBuilder SelectCheckBox(string checkBoxLabel, GumpLabelPosition labelPosition)
        {
            var processor = new SelectControlByLabelGumpParserProcessor(checkBoxLabel, labelPosition, GumpControls.CheckBox);
            var parser = new GumpParser(processor);
            parser.Parse(gump);

            if (processor.SelectedControldId.HasValue)
                return SelectCheckBox(processor.SelectedControldId.Value);

            return this;
        }

        public GumpResponseBuilder SelectCheckBox(GumpControlId id)
        {
            selectedCheckBoxes.Add(id);

            return this;
        }

        public GumpResponseBuilder SetTextEntry(string textEntryLabel, string value, GumpLabelPosition labelPosition)
        {
            var processor = new SelectControlByLabelGumpParserProcessor(textEntryLabel, labelPosition, GumpControls.TextEntry);
            var parser = new GumpParser(processor);
            parser.Parse(gump);

            if (processor.SelectedControldId.HasValue)
                textEntries.Add(new Tuple<ushort, string>((ushort)processor.SelectedControldId.Value.Value, value));

            return this;
        }
    }
}