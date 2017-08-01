using Infusion.Packets;

namespace Infusion.Gumps
{
    public interface IGumpParserProcessor
    {
        void OnButton(int x, int y, int down, int up, bool isTrigger, uint pageId, GumpControlId triggerId);
        void OnText(int x, int y, uint hue, string text);
        void OnCheckBox(int x, int y, GumpControlId id);
        void OnTextEntry(int x, int y, int width, int maxLength, string text, GumpControlId id);
    }
}