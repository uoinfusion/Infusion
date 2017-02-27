using System;

namespace Infusion.IO
{
    public interface IPullStream : IDisposable
    {
        bool DataAvailable { get; }
        int ReadByte();
        int Read(byte[] buffer, int offset, int count);
    }
}