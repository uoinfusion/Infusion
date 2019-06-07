using System;

namespace Infusion.Config
{
    public interface IConfigBagRepository
    {
        T Get<T>(string name);
        T Get<T>(string name, T defaultValue);
        T Get<T>(string name, Func<T> defaultValueProvider);
        void Update(string name, object value);
    }
}
