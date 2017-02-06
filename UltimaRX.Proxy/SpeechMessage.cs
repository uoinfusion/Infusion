using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimaRX.Packets;

namespace UltimaRX.Proxy
{
    public class SpeechMessage
    {
        public uint SpeakerId { get; set; }
        public string Message { get; set; }
        public SpeechType Type { get; set; }
        public string Name { get; set; }

        public string Text => (Name ?? string.Empty) + ": " + Message;

        public bool IsName => Message.Equals(Name, StringComparison.Ordinal);
    }
}
