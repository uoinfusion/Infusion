using System;
using System.Threading;

namespace Infusion.LegacyApi
{
    internal class Cancellation
    {
        private readonly Func<CancellationToken?> cancellationTokenProvider;

        public Cancellation(Func<CancellationToken?> cancellationTokenProvider)
        {
            this.cancellationTokenProvider = cancellationTokenProvider;
        }

        public void Check()
        {
            if (this.cancellationTokenProvider != null)
                this.cancellationTokenProvider()?.ThrowIfCancellationRequested();
        }
    }
}