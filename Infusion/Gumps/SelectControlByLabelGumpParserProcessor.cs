using System;
using Infusion.Packets;

namespace Infusion.Gumps
{
    internal sealed class SelectControlByLabelGumpParserProcessor : IProcessButton, IProcessText, IProcessCheckBox, IProcessTextEntry
    {
        private readonly string controlLabel;
        private readonly GumpLabelPosition labelPosition;
        private bool takeNextControl;
        private GumpControlId? previousControlId;
        private readonly GumpControls targetControl;

        public SelectControlByLabelGumpParserProcessor(string controlLabel, GumpLabelPosition labelPosition, GumpControls targetControl)
        {
            this.controlLabel = controlLabel;
            this.labelPosition = labelPosition;
            this.targetControl = targetControl;
        }

        public GumpControlId? SelectedControldId { get; private set; }

        void IProcessButton.OnButton(int x, int y, int down, int up, bool isTrigger, uint pageId, GumpControlId triggerId)
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

        void IProcessText.OnText(int x, int y, uint hue, string text)
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

        void IProcessCheckBox.OnCheckBox(int x, int y, GumpControlId id, int uncheckId, int checkId, bool initialState)
        {
            if (targetControl == GumpControls.CheckBox)
                ProcessControl(id);
        }

        void IProcessTextEntry.OnTextEntry(int x, int y, int width, int maxLength, string text, GumpControlId id)
        {
            if (targetControl == GumpControls.TextEntry)
                ProcessControl(id);
        }
    }
}