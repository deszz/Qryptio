using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using Qryptio.Core.Security;
using Qryptio.Core.Utility;

namespace Qryptio.Core.Exchange
{
    [Serializable]
    public class InvalidSignatureException : Exception
    {
        public InvalidSignatureException()
        { }

        public InvalidSignatureException(string message)
            : base(message)
        { }

        public InvalidSignatureException(string message, Exception inner)
            : base(message, inner)
        { }

        protected InvalidSignatureException(System.Runtime.Serialization.SerializationInfo info,
                                            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        { }
    }

    // TODO: Indicate original file name in header
    public class QrFileHeader
    {
        public long Length;

        public Version FileVersion
        { get; private set; } = new Version("0.0.0.0");
        public byte[] IV            // cipher iv
        { get; private set; }
        public byte[] Salt          // key derivation salt
        { get; private set; }
        public byte[] KeyHash       // encryption key hash
        { get; private set; }
        public int Iter             // number of key derivation iterations
        { get; private set; }
        public Dictionary<string, string> CustomFields
        { get; private set; } = new Dictionary<string, string>();

        public byte[] GetHash(byte[] key)
        {
            using (var h = System.Security.Cryptography.SHA512.Create())
                return h.ComputeHash(key);
        }

        public void SetKey(byte[] key)
        {
            KeyHash = GetHash(key);
        }

        public void WriteToStream(Stream output)
        {
            if (KeyHash == null)
                throw new InvalidOperationException("Key is not set.");

            output.Write(QrFile.MagicBytes, 0, QrFile.MagicBytes.Length);
            WriteVersionToStream(Assembly.GetAssembly(typeof(QrFileHeader)).GetName().Version, output);

            output.Write(IV);
            output.Write(Salt);
            output.Write(KeyHash);
            output.Write(BitConverter.GetBytes(Iter));

            output.WriteByte((byte)CustomFields.Count);
            foreach (var field in CustomFields)
                WriteFieldToStream(field, output);
        }

        public void ReadFromStream(Stream input)
        {
            CustomFields.Clear();

            var sig = input.ReadBytes(QrFile.MagicBytes.Length);
            if (!QrFile.MagicBytes.SequenceEqual(sig))
                throw new InvalidSignatureException();
            FileVersion = ReadVersionFromStream(input);
            // TODO: Check version

            IV = input.ReadBytes(16);
            Salt = input.ReadBytes(64);
            KeyHash = input.ReadBytes(64);
            Iter = BitConverter.ToInt32(input.ReadBytes(4), 0);

            int customFieldsCount = input.ReadByte();
            for (int i = 0; i < customFieldsCount; ++i)
            {
                var field = ReadFieldFromStream(input);
                CustomFields.Add(field.Key, field.Value);
            }

            Length = input.Position;
        }

        public void Randomize()
        {
            IV   = CryptoRandom.Bytes(16);
            Salt = CryptoRandom.Bytes(64);
            Iter = CryptoRandom.Random(256, 1024 * 4);
        }

        #region Serialization

        private void WriteVersionToStream(Version version, Stream stream)
        {
            stream.Write(BitConverter.GetBytes(version.Major));
            stream.Write(BitConverter.GetBytes(version.Minor));
            stream.Write(BitConverter.GetBytes(version.Build));
            stream.Write(BitConverter.GetBytes(version.Revision));
        }

        private void WriteFieldToStream(KeyValuePair<string, string> field, Stream stream)
        {
            stream.WriteString(field.Key, Encoding.UTF8);
            stream.WriteString(field.Value, Encoding.UTF8);
        }

        private KeyValuePair<string, string> ReadFieldFromStream(Stream stream)
        {
            var key = stream.ReadString(Encoding.UTF8);
            var val = stream.ReadString(Encoding.UTF8);

            return new KeyValuePair<string, string>(key, val);
        }

        private Version ReadVersionFromStream(Stream stream)
        {
            int major = BitConverter.ToInt32(stream.ReadBytes(4), 0);
            int minor = BitConverter.ToInt32(stream.ReadBytes(4), 0);
            int build = BitConverter.ToInt32(stream.ReadBytes(4), 0);
            int rev   = BitConverter.ToInt32(stream.ReadBytes(4), 0);

            return new Version(major, minor, build, rev);
        }

        #endregion
    }
}
