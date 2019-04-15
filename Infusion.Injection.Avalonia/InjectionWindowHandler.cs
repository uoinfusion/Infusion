using Infusion.Injection.Avalonia.InjectionObjects;
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
        public void Open(InjectionRuntime runtime, InjectionApiUO injectionApi, Legacy infusionApi)
            => InjectionWindow.Open(runtime, injectionApi, infusionApi);
        public void Open(IInjectionObjectServices objectServices)
            => InjectionWindow.Open(objectServices);
    }
}
