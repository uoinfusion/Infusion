using System;

namespace Infusion.Packets.Server
{
    internal sealed class SetWeatherPacket : MaterializedPacket
    {
        public WeatherType Type { get; set; }

        public byte NumberOfEffects { get; set; }

        public byte Temperature { get; set; }

        public override void Deserialize(Packet rawPacket)
        {
            throw new NotImplementedException();
        }

        public override Packet RawPacket
        {
            get
            {
                byte[] payload = new byte[4];
                payload[0] = (byte) PacketDefinitions.SetWeather.Id;
                payload[1] = (byte) Type;
                payload[2] = NumberOfEffects;
                payload[3] = Temperature;

                return new Packet(PacketDefinitions.SetWeather.Id, payload);
            }
        }
    }
}
