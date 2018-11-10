using InjectionScript.Interpretation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Injection
{
    internal sealed class Grabbing
    {
        private readonly Legacy api;
        private readonly InjectionHost host;
        private ObjectId? receiveingContainerId;

        public Grabbing(Legacy api, InjectionHost host)
        {
            this.api = api;
            this.host = host;
        }

        public void Grab(int amount, string id) => Grab(amount, host.GetObject(id));
        internal void Grab(string amount, string id) => Grab(NumberConversions.Str2Int(amount), host.GetObject(id));

        public void SetReceivingContainer(int id) => receiveingContainerId = (uint)id;
        public void SetReceivingContainer(string id) => receiveingContainerId = (uint)host.GetObject(id);

        public void Grab(int amount, int id)
        {
            var objId = (ObjectId)id;
            if (amount <= 0)
                amount = api.Items[objId]?.Amount ?? 0;

            api.DragItem((uint)id, amount);

            var targetContainerId = receiveingContainerId ?? api.Me.BackPack.Id;

            api.DropItem((uint)id, targetContainerId);
        }

    }
}
