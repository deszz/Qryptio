using System;
using System.Runtime.InteropServices;

namespace Qryptio.Utility
{
    internal static class NativeMethods
    {
        public enum SHSTOCKICONID : uint
        {
            SIID_SHIELD = 77
        }

        [Flags]
        public enum SHGSI : uint
        {
            SHGSI_ICON = 0x000000100,
            SHGSI_SMALLICON = 0x000000001
        }

        public enum WindowThemeAttributeType : uint
        {
            WTA_NONCLIENT = 1
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SHSTOCKICONINFO
        {
            public UInt32 cbSize;
            public IntPtr hIcon;
            public Int32 iSysIconIndex;
            public Int32 iIcon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string szPath;
        }

        public struct WTA_OPTIONS
        {
            public uint Flags;
            public uint Mask;
        }

        public const int MAX_PATH = 0x00000104;

        public const uint WTNCA_NODRAWCAPTION = 0x00000001;
        public const uint WTNCA_NODRAWICON = 0x00000002;

        public const uint MF_BYCOMMAND = 0x00000000;
        public const uint MF_GRAYED = 0x00000001;
        public const uint MF_ENABLED = 0x00000000;

        public const uint SC_CLOSE = 0xF060;

        [DllImport("gdi32.dll")]
        internal static extern bool DeleteObject(IntPtr hObject);

        [DllImport("uxtheme.dll")]
        public static extern int SetWindowThemeAttribute(IntPtr hWnd, WindowThemeAttributeType wtype, ref WTA_OPTIONS attributes, uint size);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("Shell32.dll", SetLastError = false)]
        public static extern Int32 SHGetStockIconInfo(SHSTOCKICONID siid, SHGSI uFlags, ref SHSTOCKICONINFO psii);

        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        public static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);
    }
}
