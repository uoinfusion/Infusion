using System.Collections.Generic;
using System.Text;

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

        public string DumpToString()
        {
            lock (bufferLock)
            {
                StringBuilder dumpBuilder = new StringBuilder();
                while (ringBufferQueue.Count > 0)
                {
                    dumpBuilder.AppendLine(ringBufferQueue.Dequeue());
                }

                return dumpBuilder.ToString();
            }
        }

        public void Dump()
        {
            dumpLogger.WriteLine(DumpToString());
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