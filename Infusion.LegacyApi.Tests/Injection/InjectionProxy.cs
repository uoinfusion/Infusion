using Infusion.LegacyApi.Injection;
using InjectionScript.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Tests.Injection
{
    internal sealed class InjectionProxy
    {
        public TestServerApi ServerApi { get; }
        public InfusionTestProxy TestProxy { get; }
        public InjectionApi InjectionHost { get; }
        public Player Me => TestProxy.Api.Me;

        public InjectionProxy()
        {
            TestProxy = new InfusionTestProxy();
            ServerApi = TestProxy.ServerApi;
            InjectionHost = TestProxy.Api.Injection.InjectionApi;
        }
    }
}
