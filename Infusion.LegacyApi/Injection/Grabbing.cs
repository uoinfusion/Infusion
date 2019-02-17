using InjectionScript.Runtime;
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
        private int? grabDelay;

        public Grabbing(Legacy api, InjectionHost host)
        {
            this.api = api;
            this.host = host;
        }

        public void SetReceivingContainer(int id) => receiveingContainerId = (uint)id;

        public void Grab(int amount, int id)
        {
            var objId = (ObjectId)id;
            if (amount <= 0)
                amount = api.Items[objId]?.Amount ?? 0;

            api.DragItem((uint)id, amount);

            var targetContainerId = receiveingContainerId ?? api.Me.BackPack.Id;

            api.DropItem((uint)id, targetContainerId);

            if (grabDelay.HasValue)
                api.Wait(grabDelay.Value);
        }

        public void UnsetReceivingContainer() => receiveingContainerId = null;

        internal void MoveItem(int id, int amount, int targetContainerId)
        {
            var objId = (ObjectId)id;
            if (amount <= 0)
                amount = api.Items[objId]?.Amount ?? 0;

            api.DragItem((uint)id, amount);

            if (targetContainerId <= 0)
                targetContainerId = (int)api.Me.BackPack.Id;

            api.DropItem((ObjectId)id, (ObjectId)targetContainerId);
        }

        internal void SetGrabDelay(int delay) => grabDelay = delay;
    }
}
