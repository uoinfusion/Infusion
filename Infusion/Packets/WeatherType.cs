namespace Infusion.Packets
{
    public enum WeatherType : byte
    {
        Rain = 0x00,
        FierceStorm = 0x01,
        BeginsToSnow = 0x02,
        StormIsBrewing = 0x03,
        None = 0xFF,
        NoEffect = 0xFE,
    }
}