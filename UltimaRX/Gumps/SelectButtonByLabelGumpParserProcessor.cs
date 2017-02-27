using System;

namespace Infusion.Gumps
{
    public class SelectButtonByLabelGumpParserProcessor : IGumpParserProcessor
    {
        private readonly string buttonLabel;
        private readonly GumpLabelPosition labelPosition;
        private bool takeNextButton;
        private uint? previousTriggerId;

        public SelectButtonByLabelGumpParserProcessor(string buttonLabel, GumpLabelPosition labelPosition)
        {
            this.buttonLabel = buttonLabel;
            this.labelPosition = labelPosition;
        }

        public uint? SelectedTriggerId { get; private set; }

        void IGumpParserProcessor.OnButton(int x, int y, int down, int up, bool isTrigger, uint pageId, uint triggerId)
        {
            if (SelectedTriggerId.HasValue)
                return;

            if (takeNextButton)
            {
                takeNextButton = false;
                SelectedTriggerId = triggerId;
            }
            else
            {
                previousTriggerId = triggerId;
            }
        }

        void IGumpParserProcessor.OnText(int x, int y, uint hue, string text)
        {
            if (SelectedTriggerId.HasValue)
                return;

            if (text.Equals(buttonLabel, StringComparison.CurrentCulture))
            {
                switch (labelPosition)
                {
                    case GumpLabelPosition.Before:
                        takeNextButton = true;
                        break;
                    case GumpLabelPosition.After:
                        if (previousTriggerId.HasValue)
                            SelectedTriggerId = previousTriggerId.Value;
                        break;
                }
            }

            previousTriggerId = null;
        }
    }
}