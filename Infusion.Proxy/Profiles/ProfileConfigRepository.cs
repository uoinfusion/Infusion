using Infusion.Config;
using Infusion.LegacyApi.Console;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Infusion.Desktop.Profiles
{
    public sealed class ProfileConfigRepository : IConfigBagRepository
    {
        private readonly LaunchProfile profile;

        public ProfileConfigRepository(LaunchProfile profile)
        {
            this.profile = profile;
        }

        public T Get<T>(string name) => Get(name, default(T));
        public T Get<T>(string name, T defaultValue) => Get(name, () => defaultValue);

        public T Get<T>(string name, Func<T> defaultValueProvider)
        {
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new ObjectIdConverter());
            if (profile.Options.TryGetValue(name, out var value))
            {
                if (value is JObject jobj)
                    return jobj.ToObject<T>(serializer);
                else if (typeof(T) == typeof(ObjectId))
                {
                    object result = new ObjectId((uint)Convert.ChangeType(value, typeof(uint)));
                    return (T)result;
                }

                if (value is IConvertible)
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                else
                {
                    return (T)value;
                }
            }

            return defaultValueProvider();
        }

        public void Update(string name, object value) => profile.Options[name] = value;
    }
}
