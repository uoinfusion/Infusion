using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion
{
    public class SellListItem
    {
        public ObjectId Serial;
        public ushort Amount;

        public SellListItem(ObjectId serial, ushort amount)
        {
            Serial = serial;
            Amount = amount;
        }
    }
}
