using Infusion.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Infusion.Desktop.Profiles
{
    public sealed class LaunchProfile : ICloneable, IConfigBagRepository
    {
        private readonly ProfileConfigRepository config;

        public LaunchProfile()
        {
            config = new ProfileConfigRepository(this);
        }

        public string Name { get; set; }

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public Dictionary<string, object> Options { get; internal set; } = new Dictionary<string, object>();

        public event PropertyChangedEventHandler PropertyChanged;

        object ICloneable.Clone() => Clone();

        public LaunchProfile Clone()
        {
            var newProfile = new LaunchProfile();
            newProfile.Name = this.Name;

            foreach (var optionPair in Options)
            {
                newProfile.Options[optionPair.Key] = optionPair.Value;
            }

            return newProfile;
        }

        public T Get<T>(string name) => config.Get<T>(name);
        public T Get<T>(string name, T defaultValue) => config.Get(name, defaultValue);
        public T Get<T>(string name, Func<T> defaultValueProvider) => config.Get(name, defaultValueProvider);
        public void Update(string name, object value) => config.Update(name, value);
    }
}
