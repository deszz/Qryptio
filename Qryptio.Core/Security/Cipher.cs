using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;

using Qryptio.Core.Utility;

namespace Qryptio.Core.Security
{
    public class Cipher
    {
        public static ProtectedData DeriveKey(ProtectedData password, byte[] salt, int iter, int size)
        {
            using (var _password = password.Get())
            using (var pbkdf2 = new Rfc2898DeriveBytes(_password, salt, iter))
            {
                var key = pbkdf2.GetBytes(size);

                try     { return new ProtectedData(key); }
                finally { key.Clear(); }
            }
        }

        private const CipherMode  cipherMode  = CipherMode.CBC;
        private const PaddingMode paddingMode = PaddingMode.PKCS7;
        private const int bufferSize = 4 * 1024;

        public void Encrypt(Stream input, Stream output, ProtectedData key, byte[] iv, 
                            Action<CipherProgress> progressChangedCallback, 
                            CancellationToken cancellationToken)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (output == null)
                throw new ArgumentNullException(nameof(output));
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (iv == null)
                throw new ArgumentNullException(nameof(iv));

            if (!input.CanRead)
                throw new InvalidDataException("Stream cannot be read.");
            if (!output.CanWrite)
                throw new InvalidDataException("Cannot write to stream.");

            using (var _key = key.Get())
            using (var encryptor = CreateCipher().CreateEncryptor(_key, iv))
            using (var cryptoStream = new CryptoStream(output, encryptor, CryptoStreamMode.Write))
            {
                CopyTo(input, cryptoStream, input.Position, input.Length, progressChangedCallback, cancellationToken);
                cryptoStream.FlushFinalBlock();
            }
        }

        public void Decrypt(Stream input, Stream output, ProtectedData key, byte[] iv,
                            Action<CipherProgress> progressChangedCallback, 
                            CancellationToken cancellationToken)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (output == null)
                throw new ArgumentNullException(nameof(output));
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (iv == null)
                throw new ArgumentNullException(nameof(iv));

            if (!input.CanRead)
                throw new InvalidDataException("Stream cannot be read.");
            if (!output.CanWrite)
                throw new InvalidDataException("Cannot write to stream.");

            using (var _key = key.Get())
            using (var decrypter = CreateCipher().CreateDecryptor(_key, iv))
            using (var cryptoStream = new CryptoStream(input, decrypter, CryptoStreamMode.Read))
            {
                CopyTo(cryptoStream, output, input.Position, input.Length, progressChangedCallback, cancellationToken);
            }
        }

        private SymmetricAlgorithm CreateCipher()
        {
            Aes aes = Aes.Create();

            aes.KeySize = 128;
            aes.Mode = cipherMode;
            aes.Padding = paddingMode;

            return aes;
        }
        
        private void CopyTo(Stream stream, Stream dest, long start, long end,
                            Action<CipherProgress> progressChangedCallback, 
                            CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[bufferSize];

            long pos = start;
            int read = 0;

            while ((read = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                pos += read;
                dest.Write(buffer, 0, read);

                if (progressChangedCallback != null)
                    progressChangedCallback(new CipherProgress(pos - start, end - start));

                if (cancellationToken != null)
                    cancellationToken.ThrowIfCancellationRequested();
            }
        }
    }
}
