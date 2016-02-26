using Qryptio.Core.Exchange;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Qryptio.Utility
{
    public static class Utils
    {
        public static string ConvertBinLengthToReadableString(long length)
        {
            double size;
            string unit;

            if (length < 1024)
            {
                size = length;
                unit = "B";
            }
            else if (length < 1024 * 1024)
            {
                size = (double)length / 1024;
                unit = "KB";
            }
            else if (length < 1024 * 1024 * 1024)
            {
                size = (double)length / (1024 * 1024);
                unit = "MB";
            }
            else
            {
                size = (double)length / (1024 * 1024 * 1024);
                unit = "GB";
            }

            return $"{Math.Round(size, 2)} {unit}";
        }

        public static ImageSource GetShieldIcon()
        {
            BitmapSource shieldSource = null;

            if (Environment.OSVersion.Version.Major >= 6)
            {
                NativeMethods.SHSTOCKICONINFO sii = new NativeMethods.SHSTOCKICONINFO();
                sii.cbSize = (UInt32)Marshal.SizeOf(typeof(NativeMethods.SHSTOCKICONINFO));

                Marshal.ThrowExceptionForHR(NativeMethods.SHGetStockIconInfo(
                    NativeMethods.SHSTOCKICONID.SIID_SHIELD,
                    NativeMethods.SHGSI.SHGSI_ICON | NativeMethods.SHGSI.SHGSI_SMALLICON,
                    ref sii));

                shieldSource = Imaging.CreateBitmapSourceFromHIcon(
                    sii.hIcon,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

                NativeMethods.DeleteObject(sii.hIcon);
            }
            else
            {
                shieldSource = Imaging.CreateBitmapSourceFromHIcon(
                    SystemIcons.Shield.Handle,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }

            return shieldSource;
        }

        public static ImageSource ToImageSource(this Bitmap bitmap)
        {
            lock (bitmap)
            {
                IntPtr hBitmap = bitmap.GetHbitmap();

                try
                {
                    return Imaging.CreateBitmapSourceFromHBitmap(
                        hBitmap,
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }
                finally
                {
                    NativeMethods.DeleteObject(hBitmap);
                }
            }
        }
        public static ImageSource ToImageSource(this Icon icon)
        {
            var bitmap = icon.ToBitmap();
            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            var bitmapData = bitmap.LockBits(
                rect,
                ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                var size = (rect.Width * rect.Height) * 4;

                return BitmapSource.Create(
                    bitmap.Width,
                    bitmap.Height,
                    bitmap.HorizontalResolution,
                    bitmap.VerticalResolution,
                    PixelFormats.Bgra32,
                    null,
                    bitmapData.Scan0,
                    size,
                    bitmapData.Stride);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }

        public static string GetEncryptedFileName(string plainFileName)
        {
            return plainFileName + QrFile.Extention;
        }

        public static string GetDecryptedFileName(string encryptedFileName)
        {
            if (encryptedFileName.EndsWith(QrFile.Extention))
                return encryptedFileName.Substring(0, encryptedFileName.Length - QrFile.Extention.Length);

            return encryptedFileName + ".decrypted";
        }
    }
}
