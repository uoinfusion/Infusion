using System.Collections.Generic;
using System.Text;

namespace Infusion.Proxy.Logging
{
    public sealed class RingBufferLogger : ILogger
    {
        private readonly int capacity;
        private readonly Queue<string> ringBufferQueue;
        private readonly object bufferLock = new object();

        public RingBufferLogger(int capacity)
        {
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

        public void Dump(ILogger dumpLogger)
        {
            dumpLogger.Debug(DumpToString());
        }

        public string[] Dump()
        {
            lock (bufferLock)
            {
                return ringBufferQueue.ToArray();
            }
        }

        public void Clear()
        {
            lock (bufferLock)
            {
                ringBufferQueue.Clear();
            }
        }

        public void Info(string message)
        {
            WriteLine(message);
        }

        public void Speech(SpeechMessage message)
        {
            WriteLine(message.Text);
        }

        public void Debug(string message)
        {
            WriteLine(message);
        }

        public void Critical(string message)
        {
            WriteLine(message);
        }

        public void Error(string message)
        {
            WriteLine(message);
        }
    }
}