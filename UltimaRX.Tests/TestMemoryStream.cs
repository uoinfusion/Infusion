using System;
using System.IO;

namespace UltimaRX.Tests
{
    public class TestMemoryStream : MemoryStream
    {
        public byte[] ActualBytes
        {
            get
            {
                var actualBytes = new byte[Length];

                Array.Copy(GetBuffer(), actualBytes, Length);

                return actualBytes;
            }
        }
    }
}