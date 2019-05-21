using Avalonia.Diagnostics.ViewModels;
using Avalonia.Threading;
using Infusion.Injection.Avalonia.InjectionObjects;
using InjectionScript.Runtime;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Injection.Avalonia.InjectionObjects
{
    public sealed class ObjectsViewModel : ViewModelBase
    {
        private IInjectionObjectServices objectServices;

        public ObjectsViewModel()
        {
            Objects = new ObservableCollection<ObjectItem>();

            var canActOnItem = this.WhenAnyValue(x => x.CanActOnItem);

            AddObjectCommand = ReactiveCommand.Create(AddObject);
            AddObjectFromTargetCommand = ReactiveCommand.CreateFromTask(AddObjectFromTarget, canActOnItem);
            RemoveObjectCommand = ReactiveCommand.Create(RemoveObject, canActOnItem);

            TargetCommand = ReactiveCommand.Create(Target, canActOnItem);
            UseCommand = ReactiveCommand.Create(Use, canActOnItem);
            ClickCommand = ReactiveCommand.Create(Click, canActOnItem);
            WaitTargetCommand = ReactiveCommand.Create(WaitTarget, canActOnItem);
        }

        public bool CanActOnItem
        {
            get => Objects.Any() && SelectedObject != null;
        }

        private ObservableCollection<ObjectItem> objects;
        public ObservableCollection<ObjectItem> Objects
        {
            get => objects;
            private set
            {
                RaiseAndSetIfChanged(ref objects, value);
                RaisePropertyChanged(nameof(CanActOnItem));
            }
        }

        private ObjectItem selectedObject;
        public ObjectItem SelectedObject
        {
            get => selectedObject;
            set
            {
                RaiseAndSetIfChanged(ref selectedObject, value);
                RaisePropertyChanged(nameof(SelectedObjectValue));
                RaisePropertyChanged(nameof(SelectedObjectName));
                RaisePropertyChanged(nameof(CanActOnItem));
            }
        }

        public string SelectedObjectName
        {
            get => SelectedObject?.Name ?? string.Empty;
            set
            {
                var objectId = objectServices.Get(SelectedObject.Name);
                objectServices.Remove(SelectedObject.Name);
                SelectedObject.Name = value;
                objectServices.Set(SelectedObject.Name, objectId);
                RaisePropertyChanged();
            }
        }

        private int Parse(string value) => int.Parse(value, System.Globalization.NumberStyles.HexNumber);

        public string SelectedObjectValue
        {
            get
            {
                if (SelectedObject != null)
                    return objectServices.Get(SelectedObject.Name).ToString("X8");
                else
                    return string.Empty;
            }
            set
            {
                objectServices.Set(SelectedObject.Name, Parse(value));
                RaisePropertyChanged();
            }
        }

        public ReactiveCommand<Unit, Unit> AddObjectCommand { get; }
        private void AddObject()
        {
            var i = 0;
            bool contains;
            string name;
            do
            {
                name = $"object {i}";
                contains = Objects.Any(x => x.Name == name);
                i++;
            } while (contains);

            var item = new ObjectItem(name);
            Objects.Add(item);
            objectServices.Set(name, 0);
            SelectedObject = item;
        }

        public ReactiveCommand<Unit, Unit> RemoveObjectCommand { get; }
        private void RemoveObject()
        {
            var idx = Objects.IndexOf(selectedObject);
            if (idx >= 0)
            {
                objectServices.Remove(selectedObject.Name);
                Objects.Remove(selectedObject);
                if (Objects.Count > 0)
                {
                    if (idx > 0 && idx - 1 < Objects.Count)
                        SelectedObject = Objects[idx - 1];
                    else
                        SelectedObject = Objects[0];
                }
                else
                {
                    SelectedObject = null;
                }
            }
        }

        public ReactiveCommand<Unit, Unit> AddObjectFromTargetCommand { get; }
        private async Task AddObjectFromTarget()
        {
            var targetId = await objectServices.AskForTarget();
            if (targetId != 0)
            {
                objectServices.Set(selectedObject.Name, targetId);
                SelectedObjectValue = targetId.ToString("X8");
            }
        }

        public ReactiveCommand<Unit, Unit> UseCommand { get; }
        private void Use() => objectServices.Use(SelectedObjectName);

        public ReactiveCommand<Unit, Unit> WaitTargetCommand { get; }
        private void WaitTarget() => objectServices.WaitTarget(SelectedObjectName);

        public ReactiveCommand<Unit, Unit> TargetCommand { get; }
        private void Target() => objectServices.Target(SelectedObjectName);

        public ReactiveCommand<Unit, Unit> ClickCommand { get; }
        private void Click() => objectServices.Click(SelectedObjectName);

        public IInjectionObjectServices Services
        {
            get => objectServices;
            internal set
            {
                objectServices = value;
                Objects = new ObservableCollection<ObjectItem>(objectServices.GetObjects().Select(x => new ObjectItem(x)));
                objectServices.KeyAdded += key => Dispatcher.UIThread.InvokeAsync(() => HandleAddedObject(key));
                objectServices.KeyRemoved += key => Dispatcher.UIThread.InvokeAsync(() => HandleRemoveObject(key));
            }
        }

        private void HandleAddedObject(string objectName)
        {
            if (!Objects.Any(x => x.Name.Equals(objectName, StringComparison.Ordinal)))
                Objects.Add(new ObjectItem(objectName));
        }

        private void HandleRemoveObject(string objectName)
        {
            var item = Objects.FirstOrDefault(x => x.Name.Equals(objectName, StringComparison.Ordinal));
            if (item != null)
                Objects.Remove(item);
        }
    }
}
