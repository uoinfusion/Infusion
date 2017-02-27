using System;
using Infusion.Packets;

namespace Infusion.Proxy.InjectionApi
{
    public class JournalEntry
    {
        private string text;

        public JournalEntry(string name, string message, uint speakerId, ModelId type)
        {
            Name = name;
            Message = message;
            SpeakerId = speakerId;
            Type = type;
            Created = DateTime.UtcNow;
        }

        public string Message { get; }
        public string Name { get; }

        public uint SpeakerId { get; }
        public ModelId Type { get; }

        public DateTime Created { get; }

        public string Text
        {
            get
            {
                if (text == null)
                {
                    text = $"{Name ?? "no name"}: {Message}";
                }

                return text;
            }
        }
    }
}