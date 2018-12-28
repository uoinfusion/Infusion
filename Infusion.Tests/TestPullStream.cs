using System.Collections.Generic;
using System.IO;
using Infusion.IO;

namespace Infusion.Tests
{
    public class TestPullStream : IPullStream
    {
        private readonly IEnumerator<byte[]> batchesEnumerator;
        private int position;
        private bool hasData;

        public TestPullStream(IEnumerable<byte[]> batches)
        {
            this.batchesEnumerator = batches.GetEnumerator();
            hasData = this.batchesEnumerator.MoveNext();
        }

        public void Dispose()
        {
            batchesEnumerator.Dispose();
        }

        public bool DataAvailable => hasData && position < batchesEnumerator.Current.Length;

        public int ReadByte()
        {
            return batchesEnumerator.Current[position++];
        }

        public void NextBatch()
        {
            hasData = batchesEnumerator.MoveNext();
            position = 0;
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            if (!hasData)
            {
                throw new EndOfStreamException();
            }

            int i;

            for (i = 0; i < count && position < batchesEnumerator.Current.Length; i++)
            {
                buffer[i + offset] = batchesEnumerator.Current[position++];
            }

            return i;
        }
    }
}