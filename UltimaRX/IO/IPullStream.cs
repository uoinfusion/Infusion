using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimaRX.IO
{
    public interface IPullStream : IDisposable
    {
        bool DataAvailable { get; }
        int ReadByte();
        int Read(byte[] buffer, int offset, int count);
    }
}
