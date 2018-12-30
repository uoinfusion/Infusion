using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Infusion.Packets;

namespace Infusion
{
    internal sealed class PacketHandler
    {
        private readonly List<Func<Packet, Packet?>> filters = new List<Func<Packet, Packet?>>();
        private readonly List<Func<Packet, Packet?>> outputFilters = new List<Func<Packet, Packet?>>();
        private readonly PacketDefinitionRegistry packetRegistry;

        public PacketHandler(PacketDefinitionRegistry packetRegistry)
        {
            this.packetRegistry = packetRegistry;
        }

        private ImmutableDictionary<int, ImmutableList<Delegate>> observers =
            ImmutableDictionary<int, ImmutableList<Delegate>>.Empty;

        public Packet? Filter(Packet packet)
        {
            Packet? filteredPacket = packet;

            foreach (var filter in filters)
            {
                filteredPacket = filter(filteredPacket.Value);

                if (!filteredPacket.HasValue)
                    break;
            }

            return filteredPacket;
        }

        public Packet? FilterOutput(Packet packet)
        {
            Packet? filteredPacket = packet;

            foreach (var filter in outputFilters)
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
            if (observers.TryGetValue(rawPacket.Id, out observerList))
            {
                var packet =
                    packetRegistry.Materialize<TPacket>(rawPacket);

                foreach (var observer in observerList)
                {
                    var observerAction = (Action<TPacket>) observer;
                    observerAction(packet);
                }
            }
        }

        public void RegisterFilter(Func<Packet, Packet?> filter)
        {
            filters.Add(filter);
        }

        public void Subscribe<TPacket>(PacketDefinition<TPacket> definition, Action<TPacket> observer)
            where TPacket : MaterializedPacket
        {
            ImmutableList<Delegate> observerList;
            if (!observers.TryGetValue(definition.Id, out observerList))
            {
                observerList = ImmutableList.Create((Delegate) observer);
                observers = observers.Add(definition.Id, observerList);
            }
            else
            {
                observerList = observerList.Add(observer);
                observers = observers.SetItem(definition.Id, observerList);
            }
        }

        public void Unsubscribe<TPacket>(PacketDefinition<TPacket> definition, Action<TPacket> observer)
            where TPacket : MaterializedPacket
        {
            ImmutableList<Delegate> observerList;

            if (observers.TryGetValue(definition.Id, out observerList))
            {
                observerList = observerList.Remove(observer);
                observers = observers.SetItem(definition.Id, observerList);
            }
        }

        public void RegisterOutputFilter(Func<Packet, Packet?> filter)
        {
            outputFilters.Add(filter);
        }
    }
}