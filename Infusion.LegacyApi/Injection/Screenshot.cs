using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Injection
{
    public static class Screenshot
    {
        public static string ScreenshotPath = Directory.GetCurrentDirectory();
        public static string ScreenshotPrefix = "Injection";

        private static Bitmap CaptureWindow(IntPtr handle)
        {
            var rect = new RECT();
            GetWindowRect(handle, ref rect);

            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;

            var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(bmp);
            graphics.CopyFromScreen(rect.left, rect.top, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);

            return bmp;
        }

        private static void CaptureWindowToFile(IntPtr handle, string filename, ImageFormat format)
        {
            Image img = CaptureWindow(handle);
            img.Save(filename, format);
        }

        public static void Snap(string fileName)
        {
            fileName = Path.ChangeExtension(fileName, "bmp");
            CaptureWindowToFile(UO.ClientWindow.Handle, fileName, ImageFormat.Bmp);
            UO.ClientPrint("Screenshot: " + Path.GetFileName(fileName));
        }

        public static void Snap()
        {
            int lastScreen = 0;
            int obrNum;

            DirectoryInfo dir = new DirectoryInfo(ScreenshotPath);
            if (!dir.Exists) dir.Create();
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo info in files)
            {
                string name = info.Name;
                if (name.StartsWith(ScreenshotPrefix))
                    try
                    {
                        obrNum = Convert.ToInt32(name.Substring(ScreenshotPrefix.Length, 5));
                        if (lastScreen < obrNum)
                        {
                            lastScreen = obrNum;
                        }
                    }
                    catch
                    {
                    }
            }
            lastScreen++;

            var lastScreenStr = new StringBuilder();
            for (int i = lastScreen.ToString().Length; i < 5; i++)
            {
                lastScreenStr.Append("0");
            }

            lastScreenStr.Append(lastScreen.ToString());
            string fileName = ScreenshotPrefix + lastScreenStr + ".bmp";
            string screenShotPath = Path.Combine(ScreenshotPath, fileName);

            Snap(screenShotPath);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);

        private const int SRCCOPY = 0x00CC0020; // BitBlt dwRop parameter

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
            int nWidth, int nHeight, IntPtr hObjectSource,
            int nXSrc, int nYSrc, int dwRop);
        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth,
            int nHeight);
        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hDC);
        [DllImport("gdi32.dll")]
        private static extern bool DeleteDC(IntPtr hDC);
        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);
        [DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
    }
}
