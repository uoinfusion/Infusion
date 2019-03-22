using System;
using Infusion.Packets;
using Infusion.Packets.Both;
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
        private readonly int[] lastTargetInfo = new int[4];

        public Targeting(Legacy api, InjectionHost host, IClientPacketSubject client)
        {
            this.api = api;
            this.api.TargetInfoReceived += HandleTargetInfoReceived;
            this.host = host;

            client.Subscribe(PacketDefinitions.TargetCursor, HandleClientTargetCursor);
        }

        private void HandleClientTargetCursor(TargetCursorPacket packet)
        {
            lock (targetingLock)
            {
                lastTargetInfo[0] = packet.ClickedOnType;
                lastTargetInfo[1] = packet.Location.X;
                lastTargetInfo[2] = packet.Location.Y;
                lastTargetInfo[3] = packet.Location.Z;
            }
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

        public void WaitTargetTile(int type, int x, int y, int z)
        {
            api.WaitTargetTile(type, x, y, z);
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

        internal int[] LastTile()
        {
            lock (targetingLock)
            {
                var copy = new int[4];
                lastTargetInfo.CopyTo(copy, 0);

                return copy;
            }
        }
    }
}
