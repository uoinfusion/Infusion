using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infusion.Packets;
using Infusion.Packets.Server;

namespace Infusion.LegacyApi
{
    internal sealed class SoundObserver
    {
        private readonly Configuration configuration;

        public SoundObserver(IServerPacketSubject serverPacketHandler, Configuration configuration)
        {
            this.configuration = configuration;
            serverPacketHandler.RegisterFilter(FilterBlockedSounds);
        }

        private Packet? FilterBlockedSounds(Packet rawPacket)
        {
            if (rawPacket.Id == PacketDefinitions.PlaySoundEffect.Id)
            {
                PlaySoundEffectPacket packet = null;
                var soundEffectPlayedHandler = SoundEffectPlayed;

                if (configuration.FilteredSoundSet.Any() || soundEffectPlayedHandler != null)
                    packet = PacketDefinitionRegistry.Materialize<PlaySoundEffectPacket>(rawPacket);

                if (packet != null)
                {
                    soundEffectPlayedHandler?.Invoke(this, new SoundEffectPlayedArgs(packet.Id, packet.Location));

                    if (configuration.FilteredSoundSet.Contains(packet.Id))
                        return null;
                }
            }

            return rawPacket;
        }

        public event EventHandler<SoundEffectPlayedArgs> SoundEffectPlayed;

        public void ResetEvents()
        {
            SoundEffectPlayed = null;
        }
    }
}
