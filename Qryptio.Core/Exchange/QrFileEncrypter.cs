using System;
using System.IO;
using System.Threading;

using Qryptio.Core.Security;

namespace Qryptio.Core.Exchange
{
    public class QrFileEncrypter
    {
        public EventHandler<ProgressChangedEventArgs> ProgressChanged;

        private FileInfo inputFile;
        private FileInfo outputFile;
        private QrFileHeader header;

        public QrFileEncrypter(string inputFileName, string outputFileName)
        {
            if (inputFileName == null)
                throw new ArgumentNullException(nameof(inputFileName));
            if (outputFileName == null)
                throw new ArgumentNullException(nameof(outputFileName));

            inputFile = new FileInfo(inputFileName);
            outputFile = new FileInfo(outputFileName);
            header = new QrFileHeader();

            if (!inputFile.Exists)
                throw new FileNotFoundException("Input file not found.", inputFile.FullName);

            if (String.IsNullOrWhiteSpace(outputFile.Name))
                throw new ArgumentException("Invalid output file name. Probably directory or empty path given.", nameof(outputFileName));
        }

        public void Encrypt(ProtectedData password, CancellationToken cancellationToken)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));

            try
            {
                header.Randomize();
                var key = Cipher.DeriveKey(password, header.Salt, header.Iter, 16);
                using (var _key = key.Get())
                    header.SetKey(_key);

                using (var input = new FileStream(inputFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var output = new FileStream(outputFile.FullName, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    header.WriteToStream(output);
                    new Cipher().Encrypt(input, output, key, header.IV, RaiseProgressChanged, cancellationToken);
                }
            }
            catch // TODO: Wipe output file
            {
                if (outputFile.Exists)
                    outputFile.Delete();

                throw;
            }
        }

        private void RaiseProgressChanged(CipherProgress progress)
        {
            if (ProgressChanged != null)
                ProgressChanged(this, new ProgressChangedEventArgs(progress));
        }
    }
}
