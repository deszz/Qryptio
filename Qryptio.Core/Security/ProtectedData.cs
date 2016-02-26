using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

using Qryptio.Core.Utility;

namespace Qryptio.Core
{
    public sealed class ProtectedData
    {
        public static ProtectedData Empty
        {
            get { return new ProtectedData(new byte[0]); }
        }

        private static byte[] Pad(byte[] b)
        {
            byte[] padded = new byte[16 * (((b.Length + 15) / 16) + 1)];
            b.CopyTo(padded, 0);

            return padded;
        }

        public int Length
        {
            get;
            private set;
        }

        private byte[] data;

        public ProtectedData(ProtectedData protectedData)
        {
            if (protectedData == null)
                throw new ArgumentNullException(nameof(protectedData));

            Length = protectedData.Length;
            data = (byte[])protectedData.data.Clone();
        }

        public ProtectedData(SecureString secureString)
        {
            if (secureString == null)
                throw new ArgumentNullException(nameof(secureString));

            using (var binaryString = new TemporaryArray<byte>(ToByteArray(secureString)))
            {
                Init(binaryString);
            }
        }

        public ProtectedData(string plainString)
        {
            if (plainString == null)
                throw new ArgumentNullException(nameof(plainString));

            using (var binaryString = new TemporaryArray<byte>(Encoding.UTF8.GetBytes(plainString)))
            {
                Init(binaryString);
            }
        }

        public ProtectedData(byte[] binaryArray)
        {
            if (binaryArray == null)
                throw new ArgumentNullException(nameof(binaryArray));

            Init(binaryArray);
        }

        private void Init(byte[] binaryArray)
        {
            Length = binaryArray.Length;
            data = Pad(binaryArray);

            ProtectedMemory.Protect(data, MemoryProtectionScope.SameProcess);
        }

        public TemporaryArray<byte> Get()
        {
            return new TemporaryArray<byte>(GetDataCopy());
        }

        public byte[] GetDataCopy()
        {
            byte[] unprotected = new byte[Length];

            try
            {
                ProtectedMemory.Unprotect(data, MemoryProtectionScope.SameProcess);
                Array.ConstrainedCopy(data, 0, unprotected, 0, Length);
            }
            catch
            {
                unprotected.Clear();
                throw;
            }
            finally
            {
                ProtectedMemory.Protect(data, MemoryProtectionScope.SameProcess);
            }

            return unprotected;
        }

        private unsafe byte[] ToByteArray(SecureString secureString, Encoding encoding = null)
        {
            if (secureString == null)
                throw new ArgumentNullException(nameof(secureString));
            if (encoding == null)
                encoding = Encoding.UTF8;

            int maxLength = encoding.GetMaxByteCount(secureString.Length);

            IntPtr unmanagedBytes = IntPtr.Zero;
            IntPtr unmanagedString = IntPtr.Zero;

            try
            {
                unmanagedBytes = Marshal.AllocHGlobal(maxLength);
                unmanagedString = Marshal.SecureStringToBSTR(secureString);

                char* unmanagedStringChars = (char*)unmanagedString.ToPointer();
                byte* unmanagedBytesPointer = (byte*)unmanagedBytes.ToPointer();
                int stringBytesCount = encoding.GetBytes(unmanagedStringChars, secureString.Length,
                                                         unmanagedBytesPointer, maxLength);

                byte[] bytes = new byte[stringBytesCount];
                for (int i = 0; i < stringBytesCount; ++i)
                {
                    bytes[i] = *unmanagedBytesPointer;
                    unmanagedBytesPointer++;
                }

                return bytes;
            }
            finally
            {
                if (unmanagedBytes != IntPtr.Zero)
                    Marshal.FreeHGlobal(unmanagedBytes);
                if (unmanagedString != IntPtr.Zero)
                    Marshal.ZeroFreeBSTR(unmanagedString);
            }
        }
    }
}
