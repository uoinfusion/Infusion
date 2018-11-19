using InjectionScript.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Injection
{
    internal sealed class UseSubrutines
    {
        private readonly Legacy api;

        public UseSubrutines(Legacy api)
        {
            this.api = api;
        }

        public void UseType(int type)
        {
            if (!api.TryUse((ModelId)type))
                api.ClientPrint("No item found.");
        }
    }
}
