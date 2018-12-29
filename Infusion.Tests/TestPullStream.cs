using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            Length = batches.Sum(x => x.Length);
        }

        public int Length { get; }

        public void Dispose()
        {
            batchesEnumerator.Dispose();
        }

        public bool DataAvailable => hasData && position < batchesEnumerator.Current.Length;

        public int ReadByte()
        {
            Check();

            var result = batchesEnumerator.Current[position];
            IncreasePosition();

            return result;
        }

        private void IncreasePosition()
        {
            position++;
        }

        public void NextBatch()
        {
            hasData = batchesEnumerator.MoveNext();
            position = 0;
        }

        private void Check()
        {
            if (!hasData)
            {
                throw new EndOfStreamException();
            }

            if (position >= batchesEnumerator.Current.Length)
                throw new EndOfStreamException();
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            Check();

            int i;

            for (i = 0; i < count && position < batchesEnumerator.Current.Length; i++)
            {
                buffer[i + offset] = batchesEnumerator.Current[position];
                IncreasePosition();
            }

            return i;
        }
    }
}