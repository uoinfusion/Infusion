using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Config
{
    public sealed class MemoryConfigBagRepository : IConfigBagRepository
    {
        private readonly Dictionary<string, object> storage = new Dictionary<string, object>();

        public T Get<T>(string name)
        {
            if (storage.TryGetValue(name, out object value))
                return (T)value;

            return default(T);
        }

        public void Update(string name, object value) => storage[name] = value;
    }
}
