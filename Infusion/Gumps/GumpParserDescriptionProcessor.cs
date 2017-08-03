using System.Text;
using Infusion.Packets;

namespace Infusion.Gumps
{
    internal sealed class GumpParserDescriptionProcessor : IGumpParserProcessor
    {
        private readonly StringBuilder builder = new StringBuilder();

        void IGumpParserProcessor.OnButton(int x, int y, int down, int up, bool isTrigger, uint pageId, GumpControlId triggerId)
        {
            builder.AppendLine(
                $"Button: x = {x}, y = {y}{(isTrigger ? ", isTrigger" : string.Empty)}, pageId = {pageId}, triggerId = {triggerId.Value}");
        }

        void IGumpParserProcessor.OnText(int x, int y, uint hue, string text)
        {
            builder.AppendLine($"Text: x = {x}, y = {y}, hue = {hue}, {text}");
        }

        public string GetDescription() => builder.ToString();
        public void OnCheckBox(int x, int y, GumpControlId id)
        {
            builder.AppendLine($"CheckBox: x = {x}, y = {y}, id = {id.Value}");
        }

        public void OnTextEntry(int x, int y, int width, int maxLength, string text, GumpControlId id)
        {
            builder.AppendLine(
                $"TextEntry: x = {x}, y = {y}, width = {width}, maxLength = {maxLength}, text = {text}, id = {id.Value}");
        }
    }
}