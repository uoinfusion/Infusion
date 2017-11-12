using System.Runtime.InteropServices;

namespace Infusion.LegacyApi
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WindowBounds
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}