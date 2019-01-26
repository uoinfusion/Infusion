using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infusion.Packets;
using Infusion.Packets.Client;
using Infusion.Packets.Server;

namespace Infusion.LegacyApi
{
    public class Phantoms
    {
        private readonly UltimaClient ultimaClient;
        private readonly PacketDefinitionRegistry packetRegistry;
        private readonly HashSet<ObjectId> phantomIds = new HashSet<ObjectId>();
        private readonly Dictionary<ObjectId, byte[]> trackedObjects = new Dictionary<ObjectId, byte[]>();
        private readonly object trackedObjectsLock = new object();

        internal Phantoms(UltimaClient ultimaClient, IServerPacketSubject serverPacketSubject, PacketDefinitionRegistry packetRegistry)
        {
            this.ultimaClient = ultimaClient;
            this.packetRegistry = packetRegistry;

            var clientPacketSubject = (IClientPacketSubject)ultimaClient;
            clientPacketSubject.RegisterFilter(PhantomsFilter);

            serverPacketSubject.Subscribe(PacketDefinitions.DrawObject, HandleDrawObject);
        }

        private void HandleDrawObject(DrawObjectPacket packet)
        {
            lock (trackedObjects)
            {
                if (trackedObjects.TryGetValue(packet.Id, out var payload))
                {
                    if (payload == null || payload.Length != packet.RawPacket.Length)
                        payload = new byte[packet.RawPacket.Length];

                    packet.RawPacket.Payload.CopyTo(payload, 0);
                    trackedObjects[packet.Id] = payload;
                }
            }
        }

        private Packet? PhantomsFilter(Packet rawPacket)
        {
            if (rawPacket.Id == PacketDefinitions.SingleClick.Id)
            {
                var packet = packetRegistry.Materialize<SingleClickRequest>(rawPacket);
                if (phantomIds.Contains(packet.ItemId))
                    return null;
            }

            return rawPacket;
        }

        public void Track(ObjectId id)
        {
            lock (trackedObjectsLock)
            {
                trackedObjects[id] = null;
            }
        }

        private void ShowTracked(ObjectId id, Location3D location)
        {
            byte[] payload = null;

            lock (trackedObjectsLock)
            {
                if (!trackedObjects.TryGetValue(id, out payload) || payload == null)
                {
                    return;
                }
            }

            var packet = packetRegistry.Instantiate<DrawObjectPacket>();
            packet.Deserialize(new Packet(payload[0], payload));
            packet.Location = location;
            ultimaClient.Send(packet.RawPacket);
        }

        public void ShowMobile(ObjectId id, ModelId type, Location3D location, Color color, Direction direction)
        {
            ShowTracked(id, location);
            if (id.IsMobile)
                ultimaClient.UpdatePlayer(id, type, location, direction, color);
            phantomIds.Add(id);
        }

        public void Show(ObjectId id, ModelId type, Location3D location, Color? color)
        {
            ultimaClient.ObjectInfo(id, type, location, color);
            phantomIds.Add(id);
        }

        public void Remove(ObjectId id)
        {
            if (phantomIds.Contains(id))
            {
                ultimaClient.DeleteItem(id);
                phantomIds.Remove(id);
            }
        }
    }
}
