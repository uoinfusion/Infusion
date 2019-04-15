using Avalonia;
using Avalonia.Threading;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using Infusion.LegacyApi;
using InjectionScript.Runtime;
using Infusion.Injection.Avalonia.InjectionObjects;

namespace Infusion.Injection.Avalonia
{
    public class InjectionWindow : Window
    {
        private static InjectionWindow injectionWindow;
        private static object injectionWindowLock = new object();

        public static void Open(InjectionRuntime runtime, InjectionApiUO injectionApi, Legacy infusionApi)
            => Open(new InjectionObjectServices(runtime.Objects, injectionApi, infusionApi));

        public static void Open(IInjectionObjectServices objectServices)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                lock (injectionWindowLock)
                {
                    if (injectionWindow == null)
                    {
                        injectionWindow = new InjectionWindow();
                        injectionWindow.Show();
                    }

                    injectionWindow.Objects.SetServices(objectServices);
                }
            });
        }

        public ObjectsControl Objects => this.FindControl<ObjectsControl>("Objects");

        public InjectionWindow()
        {
            this.InitializeComponent();

#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void HandleClosed()
        {
            lock (injectionWindowLock)
            {
                injectionWindow = null;
            }
        }
    }
}
