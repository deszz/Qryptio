using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Qryptio.Core.Security
{
    public static class CryptoRandom
    {
        public static double Random()
        {
            Random rand = new Random(BitConverter.ToInt32(Bytes(4), 0));
            return rand.NextDouble();
        }

        public static int Random(int min, int max)
        {
            Random rand = new Random(BitConverter.ToInt32(Bytes(4), 0));
            return rand.Next(min, max);
        }

        public static int Random(int max)
        {
            return Random(0, max);
        }

        public static T Pick<T>(IList<T> list)
        {
            return list[Random(list.Count)];
        }

        public static byte[] Bytes(int length)
        {
            byte[] bytes;
            Bytes(out bytes, length);
            return bytes;
        }

        public static void Bytes(out byte[] bytes, int length)
        {
            using (RandomNumberGenerator rand = RNGCryptoServiceProvider.Create())
            {
                bytes = new byte[length];
                rand.GetNonZeroBytes(bytes);
            }
        }
    }
}
