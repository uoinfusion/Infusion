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
        public void Open(Legacy infusionApi, InjectionApi injectionApi)
            => InjectionWindow.Open(infusionApi, injectionApi);
    }
}
