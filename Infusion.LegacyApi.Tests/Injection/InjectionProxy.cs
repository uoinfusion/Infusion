using Infusion.LegacyApi.Injection;
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
        public FindTypeSubrutine FindTypeSubrutine { get; }
        public InfusionTestProxy TestProxy { get; }
        public InjectionHost InjectionHost { get; }
        public Player Me => TestProxy.Api.Me;

        public InjectionProxy()
        {
            TestProxy = new InfusionTestProxy();
            ServerApi = TestProxy.ServerApi;
            FindTypeSubrutine = TestProxy.Api.Injection.FindTypeSubrutine;
            InjectionHost = TestProxy.Api.Injection;
        }
    }
}
