using System;
using System.Collections.Generic;
using UltimaRX.Packets;

namespace UltimaRX.Proxy
{
    public class PacketHandler
    {
        private readonly Dictionary<int, List<Delegate>> Observers = new Dictionary<int, List<Delegate>>();
        private readonly List<Func<Packet, Packet?>> Filters = new List<Func<Packet, Packet?>>();

        public Packet? Filter(Packet packet)
        {
            Packet? filteredPacket = packet;

            foreach (var filter in Filters)
            {
                filteredPacket = filter(filteredPacket.Value);

                if (!filteredPacket.HasValue)
                    break;
            }

            return filteredPacket;
        }

        public void Publish<TPacket>(Packet rawPacket) where TPacket : MaterializedPacket
        {
            List<Delegate> observerList;
            if (Observers.TryGetValue(rawPacket.Id, out observerList))
            {
                var packet =
                    PacketDefinitionRegistry.Materialize<TPacket>(rawPacket);

                foreach (var observer in observerList)
                {
                    var observerAction = (Action<TPacket>) observer;
                    observerAction(packet);
                }
            }
        }

        public void RegisterFilter(Func<Packet, Packet?> filter)
        {
            Filters.Add(filter);
        }

        public void Subscribe<TPacket>(PacketDefinition<TPacket> definition, Action<TPacket> observer)
            where TPacket : MaterializedPacket
        {
            List<Delegate> observers;
            if (!Observers.TryGetValue(definition.Id, out observers))
            {
                observers = new List<Delegate>();
                Observers.Add(definition.Id, observers);
            }
            observers.Add(observer);
        }
    }
}