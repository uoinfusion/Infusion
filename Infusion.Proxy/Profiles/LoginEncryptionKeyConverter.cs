using Infusion.IO.Encryption.Login;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Desktop.Profiles
{
    internal sealed class LoginEncryptionKeyConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(LoginEncryptionKey);
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            return new LoginEncryptionKey((uint)obj["Key1"], (uint)obj["Key2"], (uint)obj["Key3"]);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var key = (LoginEncryptionKey)value;

            writer.WriteStartObject();
            writer.WritePropertyName("Key1");
            serializer.Serialize(writer, key.Key1);
            writer.WritePropertyName("Key2");
            serializer.Serialize(writer, key.Key2);
            writer.WritePropertyName("Key3");
            serializer.Serialize(writer, key.Key3);
            writer.WriteEndObject();
        }
    }
}
