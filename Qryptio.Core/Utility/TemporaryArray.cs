using System;

namespace Qryptio.Core.Utility
{
    public sealed class TemporaryArray<T> : IDisposable
    {
        private T[] data;
        public T[] Data
        {
            get
            {
                if (disposed)
                    throw new ObjectDisposedException(nameof(TemporaryArray<T>));

                return data;
            }
        }

        public TemporaryArray(T[] array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            data = array;
        }

        public static implicit operator T[] (TemporaryArray<T> b)
        {
            return b.Data;
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            data.Clear();
            data = null;

            disposed = true;
        }

        ~TemporaryArray()
        {
            Dispose();
        }
    }
}
