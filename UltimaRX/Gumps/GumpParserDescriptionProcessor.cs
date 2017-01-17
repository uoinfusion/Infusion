using System.Text;

namespace UltimaRX.Gumps
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
    }
}