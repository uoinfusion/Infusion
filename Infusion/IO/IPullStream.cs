using System;

namespace Infusion.IO
{
    internal interface IPullStream : IDisposable
    {
        bool DataAvailable { get; }
        int ReadByte();
        int Read(byte[] buffer, int offset, int count);
    }
}