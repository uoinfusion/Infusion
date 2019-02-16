using System.Text;
using Infusion.Packets;

namespace Infusion.Gumps
{
    internal sealed class GumpParserDescriptionProcessor : IProcessButton, IProcessText, IProcessCheckBox, IProcessTextEntry, IProcessGumpPic,
        IProcessTilePicHue, IProcessButtonTileArt
    {
        private readonly StringBuilder builder = new StringBuilder();

        void IProcessButton.OnButton(int x, int y, int down, int up, bool isTrigger, uint pageId, GumpControlId triggerId)
        {
            builder.AppendLine(
                $"Button: x = {x}, y = {y}{(isTrigger ? ", isTrigger" : string.Empty)}, pageId = {pageId}, triggerId = {triggerId.Value}");
        }

        public void OnButtonTileArt(int x, int y, int width, int height, bool isTrigger, uint pageId, GumpControlId triggerId, int gumpId)
            => builder.AppendLine($"ButtonTileArt: x = {x}, y = {y}, width = {width}, height = {height}{(isTrigger ? ", isTrigger" : string.Empty)}, pageId = {pageId}, triggerId = {triggerId.Value}, gumpId = {gumpId}");


        void IProcessText.OnText(int x, int y, uint hue, string text)
        {
            builder.AppendLine($"Text: x = {x}, y = {y}, hue = {hue}, {text}");
        }

        public string GetDescription() => builder.ToString();

        void IProcessCheckBox.OnCheckBox(int x, int y, GumpControlId id, int uncheckId, int checkId, bool initialState)
        {
            builder.AppendLine($"CheckBox: x = {x}, y = {y}, id = {id.Value}, uncheckId={uncheckId}, checkId={checkId}, initialState={initialState}");
        }

        void IProcessTextEntry.OnTextEntry(int x, int y, int width, int maxLength, string text, GumpControlId id)
        {
            builder.AppendLine(
                $"TextEntry: x = {x}, y = {y}, width = {width}, maxLength = {maxLength}, text = {text}, id = {id.Value}");
        }

        void IProcessGumpPic.OnGumpPic(int x, int y, int gumpId)
        {
            builder.AppendLine($"GumpPic: x = {x}, y = {y}, gumpId = {gumpId}");
        }

        void IProcessTilePicHue.OnTilePicHue(int x, int y, uint itemId, int hue)
        {
            builder.AppendLine($"TilePicHue: x = {x}, y = {y}, itemId = {itemId}, hue = {hue}");
        }
    }
}