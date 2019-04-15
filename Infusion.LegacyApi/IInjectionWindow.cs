using InjectionScript.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.LegacyApi.Injection
{
    public interface IInjectionWindow
    {
        void Open(InjectionRuntime runtime, InjectionApiUO injectionApi, Legacy infusionApi, InjectionHost host);
    }

    internal sealed class NullInjectionWindow : IInjectionWindow
    {
        public void Open(InjectionRuntime runtime, InjectionApiUO injectionApi, Legacy infusionApi, InjectionHost host) { /*do nothing*/ }
    }
}
