using Infusion.Config;
using Infusion.LegacyApi.Console;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Infusion.Desktop.Profiles
{
    internal sealed class ProfileConfigRepository : IConfigBagRepository
    {
        private readonly LaunchProfile profile;
        private readonly IConsole console;

        public ProfileConfigRepository(LaunchProfile profile, IConsole console)
        {
            this.profile = profile;
            this.console = console;
        }

        public T Get<T>(string name)
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
                    try
                    {
                        return (T)Convert.ChangeType(value, typeof(T));
                    }
                    catch (Exception ex)
                    {
                        console.Error(ex.ToString());
                    }
                }
                else
                {
                    return (T)value;
                }
            }

            return default(T);
        }

        public void Update(string name, object value) => profile.Options[name] = value;
    }
}
