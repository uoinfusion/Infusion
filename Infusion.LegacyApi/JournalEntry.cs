using System;
using Infusion.Packets;

namespace Infusion.LegacyApi
{
    public class JournalEntry
    {
        private string text;

        public JournalEntry(long id, string name, string message, ObjectId speakerId, ModelId speakerBody, Color color,
            SpeechType type = SpeechType.Normal)
        {
            Id = id;
            Name = name;
            Message = message;
            SpeakerId = speakerId;
            SpeakerBody = speakerBody;
            Color = color;
            Created = DateTime.UtcNow;
            Type = type;
        }

        public long Id { get; }

        public string Message { get; }
        public string Name { get; }

        public ObjectId SpeakerId { get; }
        public ModelId SpeakerBody { get; }
        public SpeechType Type { get; }
        public Color Color { get; }
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