using System;
using System.IO;
using System.Text;

namespace Qryptio.Core.Utility
{
    public static class Utils
    {
        public static uint ComputeCRC32(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            uint crc = 0xFFFFFFFF;
            unchecked
            {
                int j;
                uint mask;
                for (int i = 0; i < data.Length; ++i)
                {
                    crc = crc ^ data[i];
                    for (j = 7; j >= 0; j--)
                    {
                        mask = (uint)-(crc & 1);
                        crc = (crc >> 1) ^ (0xEDB88320 & mask);
                    }
                }
            }

            return ~crc;
        }

        public static byte[] ReadBytes(this Stream stream, int count)
        {
            byte[] buffer = new byte[count];

            if (stream.Read(buffer, 0, count) < count)
                throw new EndOfStreamException();

            return buffer;
        }

        public static void Write(this Stream stream, byte[] buffer)
        {
            stream.Write(buffer, 0, buffer.Length);
        }

        public static string ReadString(this Stream stream, Encoding encoding)
        {
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            int len = BitConverter.ToInt32(stream.ReadBytes(4), 0);
            return encoding.GetString(stream.ReadBytes(len));
        }

        public static void WriteString(this Stream stream, string str, Encoding encoding)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            byte[] bin = encoding.GetBytes(str);

            stream.Write(BitConverter.GetBytes(bin.Length));
            stream.Write(bin);
        }

        public static void Clear(this Array array)
        {
            Array.Clear(array, 0, array.Length);
        }
    }
}
