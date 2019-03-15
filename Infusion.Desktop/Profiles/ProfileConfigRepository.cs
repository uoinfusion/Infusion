using Infusion.Config;
using Newtonsoft.Json.Linq;
using System;

namespace Infusion.Desktop.Profiles
{
    internal sealed class ProfileConfigRepository : IConfigBagRepository
    {
        private readonly LaunchProfile profile;

        public ProfileConfigRepository(LaunchProfile profile)
        {
            this.profile = profile;
        }

        public T Get<T>(string name)
        {
            if (profile.Options.TryGetValue(name, out var value))
            {
                if (value is JObject jobj)
                    return jobj.ToObject<T>();

                return (T)Convert.ChangeType(value, typeof(T));
            }

            return default(T);
        }

        public void Update(string name, object value) => profile.Options[name] = value;
    }
}
