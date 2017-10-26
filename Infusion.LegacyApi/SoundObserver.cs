using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infusion.LegacyApi.Events;
using Infusion.Packets;
using Infusion.Packets.Server;

namespace Infusion.LegacyApi
{
    internal sealed class SoundObserver
    {
        private readonly Configuration configuration;
        private readonly IEventJournalSource eventJournalSource;

        public SoundObserver(IServerPacketSubject serverPacketHandler, Configuration configuration, IEventJournalSource eventJournalSource)
        {
            this.configuration = configuration;
            this.eventJournalSource = eventJournalSource;
            serverPacketHandler.RegisterFilter(FilterBlockedSounds);
        }

        private Packet? FilterBlockedSounds(Packet rawPacket)
        {
            if (rawPacket.Id == PacketDefinitions.PlaySoundEffect.Id)
            {
                PlaySoundEffectPacket packet = PacketDefinitionRegistry.Materialize<PlaySoundEffectPacket>(rawPacket);

                var ev = new SoundEffectPlayedEvent(packet.Id, packet.Location);
                eventJournalSource.Publish(ev);

                if (configuration.FilteredSoundSet.Contains(packet.Id))
                    return null;
            }

            return rawPacket;
        }
    }
}
