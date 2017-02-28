using System;
using System.Collections.Generic;

namespace Infusion.Proxy.LegacyApi
{
    internal class WalkRequestQueue
    {
        private readonly Queue<WalkRequest> queue = new Queue<WalkRequest>();
        private readonly object queueLock = new object();
        private DateTime lastEnqueueTimeStamp;

        public int Count
        {
            get
            {
                lock (queueLock)
                {
                    return queue.Count;
                }
            }
        }

        public TimeSpan LastEnqueueTime
        {
            get
            {
                lock (queueLock)
                {
                    return DateTime.UtcNow - lastEnqueueTimeStamp;
                }
            }
        }

        public void Reset()
        {
            lock (queueLock)
            {
                queue.Clear();
            }
        }

        public void Enqueue(WalkRequest walkRequest)
        {
            lock (queueLock)
            {
                lastEnqueueTimeStamp = DateTime.UtcNow;
                queue.Enqueue(walkRequest);
            }
        }

        public bool TryDequeue(out WalkRequest walkRequest)
        {
            lock (queueLock)
            {
                if (queue.Count == 0)
                {
                    walkRequest = default(WalkRequest);
                    return false;
                }

                walkRequest = queue.Dequeue();
                return true;
            }
        }
    }
}