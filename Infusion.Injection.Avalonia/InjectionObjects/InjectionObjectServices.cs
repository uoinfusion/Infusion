using Infusion.LegacyApi;
using InjectionScript.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Objects = InjectionScript.Runtime.Objects;

namespace Infusion.Injection.Avalonia.InjectionObjects
{
    public sealed class InjectionObjectServices : IInjectionObjectServices
    {
        private readonly Objects injectionObjects;
        private readonly InjectionApiUO injectionApi;
        private readonly Legacy legacy;

        public event Action<string> KeyAdded
        {
            add => injectionObjects.KeyAdded += value;
            remove => injectionObjects.KeyAdded -= value;
        }

        public event Action<string> KeyRemoved
        {
            add => injectionObjects.KeyRemoved += value;
            remove => injectionObjects.KeyRemoved -= value;
        }

        public InjectionObjectServices(Objects injectionObjects, InjectionApiUO injectionApi, Legacy legacy)
        {
            this.injectionObjects = injectionObjects;
            this.injectionApi = injectionApi;
            this.legacy = legacy;
        }

        public void Set(string name, int value) => injectionObjects.Set(name, value);
        public int Get(string name) => injectionObjects.Get(name);
        public IEnumerable<string> GetObjects()
            => injectionObjects.Select(x => x.Key);
        public void Remove(string name) => injectionObjects.Remove(name);

        public void Use(string name) => injectionApi.UseObject(name);
        public void Target(string name) => legacy.Target((ObjectId)Get(name));
        public void Click(string name) => injectionApi.Click(new InjectionValue(name));
        public void WaitTarget(string name) => injectionApi.WaitTargetObject(name);
        public bool TryGet(string name, out int value) => injectionObjects.TryGet(name, out value);
        public Task<int> AskForTarget() => Task.Run(() =>
        {
            var info = legacy.Info();
            if (!info.HasValue || !info.Value.TargetsObject)
                return 0;

            return (int)info.Value.Id.Value;
        });
    }
}
