using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Injection.Avalonia.InjectionObjects
{
    public interface IInjectionObjectServices
    {
        event Action<string> KeyAdded;
        event Action<string> KeyRemoved;

        IEnumerable<string> GetObjects();
        int Get(string name);
        bool TryGet(string name, out int value);
        void Set(string name, int value);
        void Remove(string name);

        void Use(string name);
        void Target(string name);
        void Click(string name);
        void WaitTarget(string name);
        Task<int> AskForTarget();
    }
}
