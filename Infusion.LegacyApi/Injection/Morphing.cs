using Infusion.Packets;
using Infusion.Packets.Server;
using InjectionScript.Runtime;

namespace Infusion.LegacyApi.Injection
{
    internal class Morphing
    {
        private readonly Legacy api;
        private ModelId? orignalModel;
        private ModelId? morphedModel;

        public Morphing(Legacy api)
        {
            this.api = api;
            api.Server.RegisterOutputFilter(Filter);
        }

        private Packet? Filter(Packet rawPacket)
        {
            if (morphedModel.HasValue)
            {
                if (rawPacket.Id == PacketDefinitions.DrawGamePlayer.Id)
                {
                    var packet = PacketDefinitionRegistry.Materialize<DrawGamePlayerPacket>(rawPacket);
                    if (packet.PlayerId == api.Me.PlayerId)
                    {
                        packet.BodyType = morphedModel.Value;
                        packet.Serialize();

                        return packet.RawPacket;
                    }
                }
                else if (rawPacket.Id == PacketDefinitions.DrawObject.Id)
                {
                    var packet = PacketDefinitionRegistry.Materialize<DrawObjectPacket>(rawPacket);
                    if (packet.Id == api.Me.PlayerId)
                    {
                        packet.Type = morphedModel.Value;

                        return packet.RawPacket;
                    }
                }
            }

            return rawPacket;
        }

        public void Morph(int type)
        {
            ModelId model = (ushort)type;

            if (!morphedModel.HasValue)
                orignalModel = api.Me.BodyType;

            if (type <= 0)
            {
                if (orignalModel.HasValue)
                    SetPlayerModel(orignalModel.Value);
                morphedModel = null;
            }
            else
            {
                SetPlayerModel(model);
                morphedModel = model;
            }
        }

        private void SetPlayerModel(ModelId model)
            => api.Client.DrawGamePlayer(api.Me.PlayerId, model, api.Me.Location, api.Me.Direction, api.Me.MovementType, api.Me.Color);
    }
}
