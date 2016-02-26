using System.IO;
using System.Linq;
using System.Text;

using Qryptio.Core.Utility;

namespace Qryptio.Core.Exchange
{
    public static class QrFile
    {
        public static readonly byte[] MagicBytes = Encoding.ASCII.GetBytes("QR____10");
        public const string Extention  = ".qr";

        public static bool IsQrFile(string fileName)
        {
            using (var fstream = File.OpenRead(fileName))
            {
                if (fstream.Length < MagicBytes.Length)
                    return false;

                return MagicBytes.SequenceEqual(fstream.ReadBytes(MagicBytes.Length));
            }
        }
    }
}
