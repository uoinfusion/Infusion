using System;
using System.Runtime.InteropServices;

public static class Uoam
{
    private const int WM_USER = 0x400;
    private const int WM_SETMARKER = 0x400 + 303;

    public static void SetMarker(Location2D location)
        => SetMarker(location.X, location.Y);

    public static void SetMarker(int x, int y)
    {
        var hwnd = FindWindowA("CUOAMWindow", null);
        if (hwnd != IntPtr.Zero)
        {
            int lParam = x + y * 0x10000;
            SendMessage(hwnd, WM_SETMARKER, 0, lParam);
        }
    }

    [DllImport("user32")]
	public static extern IntPtr FindWindowA(string lpClassName, string lpWindowName);
	
    [DllImport("User32")]
	public static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);
}