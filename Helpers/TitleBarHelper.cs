using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace WindowsSetupTool.Helpers
{
    public static class TitleBarHelper
    {
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        private const int DWMWA_CAPTION_COLOR = 35;   // only Windows 11
        private const int DWMWA_TEXT_COLOR = 36;      // only Windows 11

        public static void Apply(Window window, Color captionColor, Color textColor)
        {
            IntPtr hwnd = new WindowInteropHelper(window).EnsureHandle();

            int dark = 1;
            DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref dark, sizeof(int));

            int caption = ToColorRef(captionColor);
            DwmSetWindowAttribute(hwnd, DWMWA_CAPTION_COLOR, ref caption, sizeof(int));

            int text = ToColorRef(textColor);
            DwmSetWindowAttribute(hwnd, DWMWA_TEXT_COLOR, ref text, sizeof(int));
        }

        // Windows needs BGR not RGB
        private static int ToColorRef(Color c) => c.R | (c.G << 8) | (c.B << 16);
    }
}