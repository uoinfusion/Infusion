using System;

namespace Infusion.IO
{
    internal interface IPushStream : IDisposable
    {
        void Write(byte[] buffer, int offset, int count);
        void WriteByte(byte value);
        void Flush();
    }
}