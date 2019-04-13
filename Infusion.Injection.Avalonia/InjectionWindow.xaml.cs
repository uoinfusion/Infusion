using Avalonia;
using Avalonia.Threading;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using Infusion.LegacyApi;
using InjectionScript.Runtime;

namespace Infusion.Injection.Avalonia
{
    public class InjectionWindow : Window
    {
        private static InjectionWindow injectionWindow;
        private static object injectionWindowLock = new object();

        public static void Open(Legacy infusionApi, InjectionApi injectionApi)
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

                    injectionWindow.injectionApi = injectionApi;
                    injectionWindow.infusionApi = infusionApi;
                }
            });
        }

        private Legacy infusionApi;
        private InjectionApi injectionApi;

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
