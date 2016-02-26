namespace Qryptio.Core.Security
{
    public struct CipherProgress
    {
        public readonly long Read;
        public readonly long Length;

        public double Percent
        {
            get { return 100 * (((double)Read) / Length); }
        }

        public CipherProgress(long read, long length)
        {
            Read = read;
            Length = length;
        }
    }
}
