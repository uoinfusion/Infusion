using Infusion.Utilities;
using System.Collections.Generic;
using System.Text;

namespace Infusion.Logging
{
    internal sealed class RingBufferLogger : ILogger
    {
        private readonly RingBuffer<string> ringBuffer;
        private readonly object bufferLock = new object();

        public RingBufferLogger(int capacity)
        {
            ringBuffer = new RingBuffer<string>(capacity);
        }

        public void WriteLine(string message)
        {
            lock (bufferLock)
            {
                ringBuffer.Add(message);
            }
        }

        public string DumpToString()
        {
            lock (bufferLock)
            {
                var dumpBuilder = new StringBuilder();
                foreach (var msg in ringBuffer)
                {
                    dumpBuilder.AppendLine(msg);
                }

                ringBuffer.Clear();
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
                return ringBuffer.ToArray();
            }
        }

        public void Clear()
        {
            lock (bufferLock)
            {
                ringBuffer.Clear();
            }
        }

        public void Info(string message)
        {
            WriteLine(message);
        }

        public void Important(string message)
        {
            WriteLine(message);
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