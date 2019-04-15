using ReactiveUI;

namespace Infusion.Injection.Avalonia.InjectionObjects
{
    public class ObjectItem : ReactiveObject
    {
        public ObjectItem(string name)
        {
            this.name = name;
        }

        private string name;
        public string Name
        {
            get => name;
            set
            {
                this.RaiseAndSetIfChanged(ref name, value);
            }
        }
    }
}
