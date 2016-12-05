using System.Collections.Generic;

namespace UltimaRX.Proxy.Logging
{
    internal sealed class RingBufferLogger : ILogger
    {
        private readonly int capacity;
        private readonly ILogger dumpLogger;
        private readonly Queue<string> ringBufferQueue;
        private readonly object bufferLock = new object();

        public RingBufferLogger(ILogger dumpLogger, int capacity)
        {
            this.dumpLogger = dumpLogger;
            this.capacity = capacity;
            ringBufferQueue = new Queue<string>(capacity);
        }

        public void WriteLine(string message)
        {
            lock (bufferLock)
            {
                if (ringBufferQueue.Count + 1 >= capacity)
                {
                    ringBufferQueue.Dequeue();
                }

                ringBufferQueue.Enqueue(message);

            }
        }

        public void Dump()
        {
            lock (bufferLock)
            {
                while (ringBufferQueue.Count > 0)
                {
                    dumpLogger.WriteLine(ringBufferQueue.Dequeue());
                }
            }
        }

        public void Clear()
        {
            lock (bufferLock)
            {
                ringBufferQueue.Clear();
            }
        }
    }
}