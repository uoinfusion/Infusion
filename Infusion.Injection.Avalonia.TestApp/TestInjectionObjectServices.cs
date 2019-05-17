using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Infusion.Injection.Avalonia.InjectionObjects;
using InjectionScript.Runtime.State;

namespace Infusion.Injection.Avalonia.TestApp
{
    internal sealed class TestInjectionObjectServices : IInjectionObjectServices
    {
        private readonly Objects objects = new Objects();

        public event Action<string> KeyAdded
        {
            add => objects.KeyAdded += value;
            remove => objects.KeyAdded -= value;
        }

        public event Action<string> KeyRemoved
        {
            add => objects.KeyRemoved += value;
            remove => objects.KeyRemoved -= value;
        }

        public void Click(string name) { }
        public int Get(string name) => objects.Get(name);
        public bool TryGet(string name, out int value) => objects.TryGet(name, out value);
        public void Remove(string name) => objects.Remove(name);
        public void Set(string name, int value) => objects.Set(name, value);
        public void Target(string name) { }
        public void Use(string name) { }
        public void WaitTarget(string name) { }

        public Task<int> AskForTarget() => Task.FromResult(1234);
        public IEnumerable<string> GetObjects() => objects.Select(x => x.Key);
    }
}
