using System;
using Infusion.Packets;

namespace Infusion.Proxy
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
