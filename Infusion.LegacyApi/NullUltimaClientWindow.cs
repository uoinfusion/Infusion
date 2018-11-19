using System;

namespace Infusion.LegacyApi
{
    public class NullUltimaClientWindow : IUltimaClientWindow
    {
        public IntPtr Handle => IntPtr.Zero;

        public void SetTitle(string title)
        {
        }

        public WindowBounds? GetBounds() => null;
        public void Focus()
        {
        }

        public void PressKey(char ch)
        {
        }

        public void PressKey(KeyCode keyCode)
        {
        }

        public void SendText(string text)
        {
        }

        public void Click(int x, int y)
        {
        }
    }
}