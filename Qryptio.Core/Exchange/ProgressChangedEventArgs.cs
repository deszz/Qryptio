using System;

using Qryptio.Core.Security;

namespace Qryptio.Core.Exchange
{
    public class ProgressChangedEventArgs : EventArgs
    {
        public CipherProgress Progress;

        public ProgressChangedEventArgs(CipherProgress progress)
        {
            Progress = progress;
        }
    }
}
