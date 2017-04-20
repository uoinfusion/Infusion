using System;

namespace Infusion.Gumps
{
    public class SelectControlByLabelGumpParserProcessor : IGumpParserProcessor
    {
        private readonly string controlLabel;
        private readonly GumpLabelPosition labelPosition;
        private bool takeNextControl;
        private uint? previousControlId;
        private GumpControls targetControl;

        public SelectControlByLabelGumpParserProcessor(string controlLabel, GumpLabelPosition labelPosition, GumpControls targetControl)
        {
            this.controlLabel = controlLabel;
            this.labelPosition = labelPosition;
            this.targetControl = targetControl;
        }

        public uint? SelectedControldId { get; private set; }

        void IGumpParserProcessor.OnButton(int x, int y, int down, int up, bool isTrigger, uint pageId, uint triggerId)
        {
            if (targetControl == GumpControls.Button)
                ProcessControl(triggerId);
        }

        private void ProcessControl(uint controlId)
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

        void IGumpParserProcessor.OnCheckBox(int x, int y, uint id)
        {
            if (targetControl == GumpControls.CheckBox)
                ProcessControl(id);
        }

        public void OnTextEntry(int x, int y, int width, int maxLength, string text, uint id)
        {
            if (targetControl == GumpControls.TextEntry)
                ProcessControl(id);
        }
    }
}