using System;
using Infusion.Packets;

namespace Infusion.LegacyApi
{
    public class JournalEntry
    {
        private string text;

        public JournalEntry(long id, string name, string message, ObjectId speakerId, ModelId type)
        {
            Id = id;
            Name = name;
            Message = message;
            SpeakerId = speakerId;
            Type = type;
            Created = DateTime.UtcNow;
        }

        public long Id { get; }

        public string Message { get; }
        public string Name { get; }

        public ObjectId SpeakerId { get; }
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

        public override string ToString()
            => $"{Id}, {Created:MM/dd/yyyy HH:mm:ss:fffff}: {Text}";
    }
}