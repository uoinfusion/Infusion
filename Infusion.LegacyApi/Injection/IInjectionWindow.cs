using InjectionScript.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.LegacyApi.Injection
{
    public interface IInjectionWindow
    {
        void Open(Legacy infusionApi, InjectionApi injectionApi);
    }

    internal sealed class NullInjectionWindow : IInjectionWindow
    {
        public void Open(Legacy infusionApi, InjectionApi injectionApi) { /*do nothing*/ }
    }
}
