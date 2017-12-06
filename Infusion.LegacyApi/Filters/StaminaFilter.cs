using Infusion.IO;
using Infusion.Packets;
using Infusion.Packets.Server;

namespace Infusion.LegacyApi.Filters
{
    internal class StaminaFilter : IStaminaFilter
    {
        private ushort? fakedStamina = null;
        private readonly UltimaClient ultimaClient;

        private ushort lastCurrentStamina;
        private ushort lastMaxStamina;
        private ObjectId? lastPlayerId;

        internal StaminaFilter(IServerPacketSubject serverPacketHandler, UltimaClient ultimaClient)
        {
            serverPacketHandler.RegisterOutputFilter(FilterStamina);
            this.ultimaClient = ultimaClient;
            serverPacketHandler.Subscribe(PacketDefinitions.UpdateCurrentStamina, HandleUpdateCurrentStamina);
            serverPacketHandler.Subscribe(PacketDefinitions.StatusBarInfo, HandleStatusBarInfo);
        }

        private void HandleStatusBarInfo(StatusBarInfoPacket packet)
        {
            lastCurrentStamina = packet.CurrentStamina;
            lastMaxStamina = packet.MaxStamina;
            lastPlayerId = packet.PlayerId;
        }

        private void HandleUpdateCurrentStamina(UpdateCurrentStaminaPacket packet)
        {
            lastCurrentStamina = packet.CurrentStamina;
            lastMaxStamina = packet.MaxStamina;
            lastPlayerId = packet.PlayerId;
        }

        private Packet? FilterStamina(Packet rawPacket)
        {
            ushort? stamina = fakedStamina;

            if (stamina.HasValue)
            {
                if (rawPacket.Id == PacketDefinitions.UpdateCurrentStamina.Id)
                {
                    var writer = new ArrayPacketWriter(rawPacket.Payload);
                    writer.Position = 7;
                    writer.WriteUShort(stamina.Value);
                }
                if (rawPacket.Id == PacketDefinitions.StatusBarInfo.Id)
                {
                    // hack: NPC status bar packets are shorter (doesn't contain other stats than HP),
                    // don't want to materialize specific packet and check mobile id, so
                    // packet lenght check could be sufficient
                    if (rawPacket.Length > 50)
                    {
                        var writer = new ArrayPacketWriter(rawPacket.Payload);
                        writer.Position = 50;
                        writer.WriteUShort(stamina.Value);
                    }
                }
            }

            return rawPacket;
        }

        public void SetFakeStamina(ushort stamina)
        {
            fakedStamina = stamina;
        }

        public void Disable()
        {
            fakedStamina = null;

            if (lastPlayerId.HasValue)
            {
                ultimaClient.UpdateCurrentStamina(lastPlayerId.Value, lastCurrentStamina, lastMaxStamina);
            }
        }
    }
}
