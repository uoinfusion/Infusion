using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Encoder = System.Drawing.Imaging.Encoder;

namespace UltimaRX.Nazghul.Proxy
{
    public static class ScreenshotHelpers
    {
        public static void TakeScreenshot(string processName, Stream targetStream)
        {
            var screenshot = TakeScreenshot(processName);

            var jpgEncoder = ImageCodecInfo.GetImageDecoders().First(x => x.FormatID == ImageFormat.Jpeg.Guid);
            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 15L);
            screenshot.Save(targetStream, jpgEncoder, encoderParameters);
        }

        public static void TakeScreenshot(string processName, string fileName)
        {
            using (var stream = File.Create(fileName))
            {
                TakeScreenshot(processName, stream);
                stream.Flush();
            }
        }

        public static Bitmap TakeScreenshot(string processName)
        {
            var process = Process.GetProcessesByName(processName)[0];
            User32.SetForegroundWindow(process.MainWindowHandle);

            var rect = new User32.Rect();
            User32.GetWindowRect(process.MainWindowHandle, ref rect);

            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;

            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(rect.left, rect.top, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);

            return bitmap;
        }

        private class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct Rect
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }

            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool SetForegroundWindow(IntPtr hWnd);
        }
    }
}
