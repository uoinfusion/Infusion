using Avalonia;
using Avalonia.Threading;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using Infusion.LegacyApi;
using InjectionScript.Runtime;
using Infusion.Injection.Avalonia.InjectionObjects;
using Infusion.Injection.Avalonia.Scripts;
using Infusion.LegacyApi.Injection;

namespace Infusion.Injection.Avalonia
{
    public class InjectionWindow : Window
    {
        private static InjectionWindow injectionWindow;
        private static object injectionWindowLock = new object();

        public static void Open(InjectionRuntime runtime, InjectionApiUO injectionApi, Legacy infusionApi, InjectionHost host)
            => Open(new InjectionObjectServices(runtime.Objects, injectionApi, infusionApi), new ScriptServices(runtime, host));

        public static void Open(IInjectionObjectServices objectServices, IScriptServices scriptServices)
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
                    injectionWindow.Scripts.SetServices(scriptServices);
                }
            });
        }

        public ObjectsControl Objects => this.FindControl<ObjectsControl>("Objects");
        public ScriptsControl Scripts => this.FindControl<ScriptsControl>("Scripts");

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
