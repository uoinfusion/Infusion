using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion
{
    public class SellListItem
    {
        internal ObjectId Serial;
        internal ushort Amount;

        internal SellListItem(ObjectId s, ushort a)
        {
            Serial = s;
            Amount = a;
        }
    }
}
