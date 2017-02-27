using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Infusion.Packets;

namespace Infusion.Proxy
{
    public class PacketHandler
    {
        private ImmutableDictionary<int, ImmutableList<Delegate>> Observers = ImmutableDictionary<int, ImmutableList<Delegate>>.Empty;
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
            ImmutableList<Delegate> observerList;
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
            ImmutableList<Delegate> observerList;
            if (!Observers.TryGetValue(definition.Id, out observerList))
            {
                observerList = ImmutableList.Create((Delegate)observer);
                Observers = Observers.Add(definition.Id, observerList);
            }
            else
            {
                observerList = observerList.Add(observer);
                Observers = Observers.SetItem(definition.Id, observerList);
            }
        }

        public void Unsubscribe<TPacket>(PacketDefinition<TPacket> definition, Action<TPacket> observer)
            where TPacket : MaterializedPacket
        {
            ImmutableList<Delegate> observerList;

            if (Observers.TryGetValue(definition.Id, out observerList))
            {
                observerList = observerList.Remove(observer);
                Observers = Observers.SetItem(definition.Id, observerList);
            }
        }
    }
}