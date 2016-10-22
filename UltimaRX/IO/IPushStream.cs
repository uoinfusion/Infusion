using System;

namespace UltimaRX.IO
{
    public interface IPushStream : IDisposable
    {
        void Write(byte[] buffer, int offset, int count);
    }
}