using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Utilities
{
    public class RingBuffer<T> : IEnumerable<T>
    {
        private readonly Queue<T> ringBufferQueue;
        private readonly int capacity;

        public RingBuffer(int capacity)
        {
            this.capacity = capacity;
            ringBufferQueue = new Queue<T>(capacity);
        }

        public void Add(T element)
        {
            if (ringBufferQueue.Count + 1 >= capacity)
            {
                ringBufferQueue.Dequeue();
            }

            ringBufferQueue.Enqueue(element);
        }

        public IEnumerator<T> GetEnumerator() => ringBufferQueue.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ringBufferQueue.GetEnumerator();

        public T[] ToArray() => ringBufferQueue.ToArray();
        public void Clear() => ringBufferQueue.Clear();
        public int Count => ringBufferQueue.Count;
    }
}
