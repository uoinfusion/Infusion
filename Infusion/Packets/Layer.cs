namespace Infusion.Packets
{
    public enum Layer : byte
    {
        Unknown = 0x00,
        OneHandedWeapon = 0x01,
        TwoHandedWeapon = 0x02,
        Shoes = 0x03,
        Pants = 0x04,
        Shirt = 0x05,
        Helm = 0x06,
        Gloves = 0x07,
        Ring = 0x08,
        Neck = 0x0A,
        Hair = 0x0B,
        Waist = 0x0C,
        Torso = 0x0D,
        Bracelet = 0x0E,
        FacialHair = 0x10,
        TorsoMiddle = 0x11,
        Earrings = 0x12,
        Arms = 0x13,
        Back = 0x14,
        Backpack = 0x15,
        TorsoOuter = 0x16,
        LegsOuter = 0x17,
        LegsInner = 0x18,
        Mount = 0x19,
        RestockContainer = 0x1A,
        NoRestockContainer = 0x1B,
        SellContainer = 0x1C,
        BankBox = 0x1D
    }
}