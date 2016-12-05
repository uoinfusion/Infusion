using System;

namespace UltimaRX.Packets
{
    [Flags]
    public enum ObjectFlag : byte
    {
        None = 0x00,
        Female = 0x02,
        Poisoned = 0x04,
        YellowHits = 0x08,
        FactionShip = 0x10,
        Movable = 0x20,
        WarMode = 0x40,
        Hidden = 0x80,
    }
}