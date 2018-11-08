using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi
{
    public interface IUltimaClientWindow
    {
        IntPtr Handle { get; }

        void SetTitle(string title);
        WindowBounds? GetBounds();
        void Focus();
        void PressKey(char ch);
        void PressKey(KeyCode keyCode);
        void SendText(string text);
    }
}
