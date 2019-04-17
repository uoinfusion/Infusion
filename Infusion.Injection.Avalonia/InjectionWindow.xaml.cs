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
        private readonly InjectionWindowConfiguration configuration;

        public static void Open(InjectionRuntime runtime, InjectionApiUO injectionApi, Legacy infusionApi, InjectionHost host)
            => Open(new InjectionObjectServices(runtime.Objects, injectionApi, infusionApi), new ScriptServices(runtime, host), new InjectionWindowConfiguration(infusionApi.Config));

        public static void Open(IInjectionObjectServices objectServices, IScriptServices scriptServices, InjectionWindowConfiguration configuration)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                lock (injectionWindowLock)
                {
                    if (injectionWindow == null)
                    {
                        injectionWindow = new InjectionWindow(configuration);
                        injectionWindow.Show();
                    }

                    injectionWindow.Objects.SetServices(objectServices);
                    injectionWindow.Scripts.SetServices(scriptServices);
                }
            });
        }

        public ObjectsControl Objects => this.FindControl<ObjectsControl>("Objects");
        public ScriptsControl Scripts => this.FindControl<ScriptsControl>("Scripts");

        public InjectionWindow(InjectionWindowConfiguration configuration)
        { 
            this.configuration = configuration;

            this.InitializeComponent();

#if DEBUG
            this.AttachDevTools();
#endif

            this.Position = new PixelPoint(configuration.WindowX, configuration.WindowY);
            this.PositionChanged += (sender, e) =>
            {
                this.configuration.WindowX = e.Point.X;
                this.configuration.WindowY = e.Point.Y;
            };
        }

        private void InjectionWindow_PositionChanged(object sender, PixelPointEventArgs e) => throw new NotImplementedException();

        protected override bool HandleClosing()
        {
            this.configuration.WindowX = Position.X;
            this.configuration.WindowY = Position.Y;

            this.configuration.Save();
            return base.HandleClosing();
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
