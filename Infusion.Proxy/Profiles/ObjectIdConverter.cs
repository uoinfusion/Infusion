using System;
using Newtonsoft.Json;

namespace Infusion.Desktop.Profiles
{
    internal class ObjectIdConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(ObjectId);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.ValueType == typeof(long))
                return new ObjectId((uint)(long)reader.Value);
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((ObjectId)value).Value);
        }
    }
}