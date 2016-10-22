using System.Collections.Generic;

namespace UltimaRX.IO
{
    public class MemoryBatchedStream : IPullStream
    {
        private readonly IEnumerator<byte[]> contentEnumerator;
        private int position;

        public MemoryBatchedStream(IEnumerable<byte[]> content)
        {
            contentEnumerator = content.GetEnumerator();
            MoveNext();
        }

        public bool DataAvailable { get; private set; }

        public int ReadByte()
        {
            if (!DataAvailable)
                return -1;

            var result = contentEnumerator.Current[position];

            IncrementPosition();

            return result;
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            for (var i = offset; i < offset + count; i++)
            {
                var value = ReadByte();
                if ((value < 0) || (value > 255))
                    return i - offset;

                buffer[i] = (byte) value;
            }

            return count;
        }

        public void Dispose()
        {
            contentEnumerator?.Dispose();
        }

        private bool MoveNext()
        {
            if (contentEnumerator.MoveNext())
            {
                DataAvailable = true;
                if (contentEnumerator.Current.Length == 0)
                    return MoveNext();

                position = 0;
                return true;
            }

            position = -1;
            DataAvailable = false;
            return false;
        }

        private void IncrementPosition()
        {
            position++;
            if (position >= contentEnumerator.Current.Length)
                MoveNext();
        }
    }
}