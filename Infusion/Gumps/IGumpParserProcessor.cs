namespace Infusion.Gumps
{
    public interface IGumpParserProcessor
    {
        void OnButton(int x, int y, int down, int up, bool isTrigger, uint pageId, uint triggerId);
        void OnText(int x, int y, uint hue, string text);
    }
}