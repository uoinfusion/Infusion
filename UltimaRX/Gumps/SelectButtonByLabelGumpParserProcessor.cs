using System;

namespace UltimaRX.Gumps
{
    public class SelectButtonByLabelGumpParserProcessor : IGumpParserProcessor
    {
        private readonly string buttonLabel;
        private bool takeNextButton;

        public SelectButtonByLabelGumpParserProcessor(string buttonLabel)
        {
            this.buttonLabel = buttonLabel;
        }

        public uint? SelectedTriggerId { get; private set; }

        void IGumpParserProcessor.OnButton(int x, int y, int down, int up, bool isTrigger, uint pageId, uint triggerId)
        {
            if (takeNextButton)
            {
                takeNextButton = false;
                SelectedTriggerId = triggerId;
            }
        }

        void IGumpParserProcessor.OnText(int x, int y, uint hue, string text)
        {
            if (text.Equals(buttonLabel, StringComparison.CurrentCulture))
            {
                takeNextButton = true;
            }
        }
    }
}