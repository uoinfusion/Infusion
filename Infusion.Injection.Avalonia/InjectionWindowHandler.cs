using Infusion.Injection.Avalonia.InjectionObjects;
using Infusion.Injection.Avalonia.Scripts;
using Infusion.LegacyApi;
using Infusion.LegacyApi.Injection;
using InjectionScript.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.Injection.Avalonia
{
    public sealed class InjectionWindowHandler : IInjectionWindow
    {
        public void Open(InjectionRuntime runtime, InjectionApiUO injectionApi, Legacy infusionApi, InjectionHost host)
            => InjectionWindow.Open(runtime, injectionApi, infusionApi, host);
        public void Open(IInjectionObjectServices objectServices, IScriptServices scriptServices, InjectionWindowConfiguration configuration)
            => InjectionWindow.Open(objectServices, scriptServices, configuration);
    }
}
