using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;

using Qryptio.Core.Security;

namespace Qryptio.Core.Exchange
{

    [Serializable]
    public class FileCorruptedException : Exception
    {
        public FileCorruptedException()
        { }

        public FileCorruptedException(string message) 
            : base(message)
        { }

        public FileCorruptedException(string message, Exception inner) 
            : base(message, inner)
        { }

        protected FileCorruptedException(System.Runtime.Serialization.SerializationInfo info,
                                         System.Runtime.Serialization.StreamingContext context) 
            : base(info, context)
        { }
    }

    [Serializable]
    public class InvalidPasswordException : Exception
    {
        public InvalidPasswordException()
        { }

        public InvalidPasswordException(string message)
            : base(message)
        { }

        public InvalidPasswordException(string message, Exception inner)
            : base(message, inner)
        { }

        protected InvalidPasswordException(System.Runtime.Serialization.SerializationInfo info,
                                           System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        { }
    }

    public class QrFileDecrypter
    {
        public EventHandler<ProgressChangedEventArgs> ProgressChanged;

        private FileInfo inFile;
        private FileInfo outFile;
        private QrFileHeader header;

        public QrFileDecrypter(string inputFileName, string outputFileName)
        {
            if (inputFileName == null)
                throw new ArgumentNullException(nameof(inputFileName));
            if (outputFileName == null)
                throw new ArgumentNullException(nameof(outputFileName));

            inFile = new FileInfo(inputFileName);
            outFile = new FileInfo(outputFileName);
            header = new QrFileHeader();

            if (String.IsNullOrWhiteSpace(outFile.Name))
                throw new ArgumentException("Invalid output file name. Probably directory or empty path given.", nameof(outputFileName));

            using (var stream = new FileStream(inFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                header.ReadFromStream(stream);
        }

        public void Decrypt(ProtectedData password, CancellationToken cancellationToken)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));

            var tmp = outFile.FullName + ".tmp";
            var key = Cipher.DeriveKey(password, header.Salt, header.Iter, 16);

            using (var _key = key.Get())
            {
                if (!header.GetHash(_key).SequenceEqual(header.KeyHash))
                    throw new InvalidPasswordException("Invalid key hash.");
            }

            try
            {
                using (var input = new FileStream(inFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var output = new FileStream(tmp, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                {
                    input.Seek(header.Length, SeekOrigin.Begin);

                    try
                    {
                        new Cipher().Decrypt(input, output, key, header.IV, RaiseProgressChanged, cancellationToken);
                    }
                    catch (CryptographicException e)
                    {
                        throw new FileCorruptedException("Cryptographic Exception.", e);
                    }
                }

                if (outFile.Exists)
                    outFile.Delete();

                File.Move(tmp, outFile.FullName);

            }
            catch (EndOfStreamException e)
            {
                if (File.Exists(tmp))
                    File.Delete(tmp);

                throw new FileCorruptedException("End of stream.", e);
            }
            catch
            {
                if (File.Exists(tmp))
                    File.Delete(tmp);

                throw;
            }
        }

        // TODO: Check percent change to avoid useless event calls
        private void RaiseProgressChanged(CipherProgress progress)
        {
            if (ProgressChanged != null)
                ProgressChanged(this, new ProgressChangedEventArgs(progress));
        }
    }
}
