using Infusion.Packets.Client;
using InjectionScript.Runtime;

namespace Infusion.LegacyApi.Injection
{
    internal sealed class Targeting
    {
        private readonly Legacy api;
        private readonly InjectionHost host;
        private string currentObjectName;
        private readonly object targetingLock = new object();

        public Targeting(Legacy api, InjectionHost host)
        {
            this.api = api;
            this.api.TargetInfoReceived += HandleTargetInfoReceived;
            this.host = host;
        }

        private void HandleTargetInfoReceived(TargetInfo? obj)
        {
            lock (targetingLock)
            {
                if (currentObjectName != null && obj.HasValue && obj.Value.TargetsObject && obj.Value.Id.HasValue)
                {
                    host.AddObject(currentObjectName, (int)obj.Value.Id.Value);
                }

                currentObjectName = null;
            }
        }

        public void WaitTargetObject(ObjectId id)
        {
            api.WaitTargetObject(id);
        }

        public void WaitTargetObject(ObjectId id1, ObjectId id2)
        {
            api.WaitTargetObject(id1, id2);
        }

        public void AddObject(string objectName)
        {
            lock (targetingLock)
            {
                currentObjectName = objectName;
            }

            api.AskForTarget();
        }

        public bool IsTargeting
        {
            get
            {
                lock (targetingLock)
                {
                    return currentObjectName != null;
                }
            }
        }
    }
}
