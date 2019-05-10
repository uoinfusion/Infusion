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
using Infusion.Injection.Avalonia.Main;
using ReactiveUI;

namespace Infusion.Injection.Avalonia
{
    public class InjectionWindow : Window
    {
        private static InjectionWindow injectionWindow;
        private static object injectionWindowLock = new object();
        private readonly InjectionConfiguration configuration;

        public static void Open(InjectionRuntime runtime, InjectionApiUO injectionApi, Legacy infusionApi, InjectionHost host)
            => Open(new InjectionObjectServices(runtime.Objects, injectionApi, infusionApi), new ScriptServices(runtime, host), new MainServices(infusionApi, host),
                new InjectionConfiguration(infusionApi.Config, host.InjectionOptions));

        public static void Open(IInjectionObjectServices objectServices, IScriptServices scriptServices, IMainServices mainServices, InjectionConfiguration configuration)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                lock (injectionWindowLock)
                {
                    if (injectionWindow == null)
                    {
                        injectionWindow = new InjectionWindow(configuration);
                    }

                    injectionWindow.Objects.SetServices(objectServices);
                    injectionWindow.Scripts.SetServices(scriptServices);
                    injectionWindow.Main.ViewModel.SetServices(injectionWindow.configuration, mainServices);

                    injectionWindow.Show();
                }
            });
        }

        public ObjectsControl Objects => this.FindControl<ObjectsControl>("Objects");
        public ScriptsControl Scripts => this.FindControl<ScriptsControl>("Scripts");
        public MainControl Main => this.FindControl<MainControl>("Main");

        public InjectionWindow(InjectionConfiguration configuration)
        { 
            this.configuration = configuration;

            this.InitializeComponent();

#if DEBUG
            this.AttachDevTools();
#endif

            this.Position = new PixelPoint(configuration.Window.X, configuration.Window.Y);
            this.Topmost = configuration.Window.AlwaysOnTop;

            this.PositionChanged += (sender, e) =>
            {
                // When user minimizes Avalonia windows then PositionChanged event gets executed first even before changing WindowState property and
                // position of the window is -32000, -32000.
                if (this.Position.X >= 0 && this.Position.Y >= 0)
                {
                    this.configuration.Window.X = this.Position.X;
                    this.configuration.Window.Y = this.Position.Y;
                }
            };

            Main.ViewModel.WhenAnyValue(x => x.AlwaysOnTop).Subscribe(alwaysOnTop => this.Topmost = alwaysOnTop);
        }

        protected override bool HandleClosing()
        {
            this.configuration.Window.X = Position.X;
            this.configuration.Window.Y = Position.Y;

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
