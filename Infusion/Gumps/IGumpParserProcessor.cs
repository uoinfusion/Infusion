using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Gumps
{
    public interface IGumpParserProcessor
    {
    }

    public interface IProcessButton : IGumpParserProcessor
    {
        void OnButton(int x, int y, int down, int up, bool isTrigger, uint pageId, GumpControlId triggerId);
    }

    public interface IProcessText : IGumpParserProcessor
    {
        void OnText(int x, int y, uint hue, string text);
    }

    public interface IProcessCheckBox : IGumpParserProcessor
    {
        void OnCheckBox(int x, int y, GumpControlId id, int uncheckId, int checkId, bool initialState);
    }

    public interface IProcessTextEntry : IGumpParserProcessor
    {
        void OnTextEntry(int x, int y, int width, int maxLength, string text, GumpControlId id);
    }

    public interface IProcessTilePicHue : IGumpParserProcessor
    {
        void OnTilePicHue(int x, int y, uint itemId, int hue);
    }

    public interface IProcessGumpPic : IGumpParserProcessor
    {
        void OnGumpPic(int x, int y, int gumpId);
    }
}
