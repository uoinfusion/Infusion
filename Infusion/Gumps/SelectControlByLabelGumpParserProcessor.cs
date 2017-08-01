using System;
using Infusion.Packets;

namespace Infusion.Gumps
{
    public class SelectControlByLabelGumpParserProcessor : IGumpParserProcessor
    {
        private readonly string controlLabel;
        private readonly GumpLabelPosition labelPosition;
        private bool takeNextControl;
        private GumpControlId? previousControlId;
        private GumpControls targetControl;

        public SelectControlByLabelGumpParserProcessor(string controlLabel, GumpLabelPosition labelPosition, GumpControls targetControl)
        {
            this.controlLabel = controlLabel;
            this.labelPosition = labelPosition;
            this.targetControl = targetControl;
        }

        public GumpControlId? SelectedControldId { get; private set; }

        void IGumpParserProcessor.OnButton(int x, int y, int down, int up, bool isTrigger, uint pageId, GumpControlId triggerId)
        {
            if (targetControl == GumpControls.Button)
                ProcessControl(triggerId);
        }

        private void ProcessControl(GumpControlId controlId)
        {
            if (SelectedControldId.HasValue)
                return;

            if (takeNextControl)
            {
                takeNextControl = false;
                SelectedControldId = controlId;
            }
            else
            {
                previousControlId = controlId;
            }
        }

        void IGumpParserProcessor.OnText(int x, int y, uint hue, string text)
        {
            if (SelectedControldId.HasValue)
                return;

            if (text.Equals(controlLabel, StringComparison.CurrentCulture))
            {
                switch (labelPosition)
                {
                    case GumpLabelPosition.Before:
                        takeNextControl = true;
                        break;
                    case GumpLabelPosition.After:
                        if (previousControlId.HasValue)
                            SelectedControldId = previousControlId.Value;
                        break;
                }
            }

            previousControlId = null;
        }

        void IGumpParserProcessor.OnCheckBox(int x, int y, GumpControlId id)
        {
            if (targetControl == GumpControls.CheckBox)
                ProcessControl(id);
        }

        public void OnTextEntry(int x, int y, int width, int maxLength, string text, GumpControlId id)
        {
            if (targetControl == GumpControls.TextEntry)
                ProcessControl(id);
        }
    }
}