using System.Text;

namespace Infusion.Gumps
{
    public class GumpParserDescriptionProcessor : IGumpParserProcessor
    {
        private readonly StringBuilder builder = new StringBuilder();

        void IGumpParserProcessor.OnButton(int x, int y, int down, int up, bool isTrigger, uint pageId, uint triggerId)
        {
            builder.AppendLine(
                $"Button: x = {x}, y = {y}{(isTrigger ? ", isTrigger" : string.Empty)}, pageId = {pageId}, triggerId = {triggerId}");
        }

        void IGumpParserProcessor.OnText(int x, int y, uint hue, string text)
        {
            builder.AppendLine($"Text: x = {x}, y = {y}, hue = {hue}, {text}");
        }

        public string GetDescription() => builder.ToString();
        public void OnCheckBox(int x, int y, uint id)
        {
            builder.AppendLine($"CheckBox: x = {x}, y = {y}, id = {id}");
        }

        public void OnTextEntry(int x, int y, int width, int maxLength, string text, uint id)
        {
            builder.AppendLine(
                $"TextEntry: x = {x}, y = {y}, width = {width}, maxLength = {maxLength}, text = {text}, id = {id}");
        }
    }
}