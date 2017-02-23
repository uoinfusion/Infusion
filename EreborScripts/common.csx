using System;
using System.Collections.Generic;
using System.Threading;
using UltimaRX.Proxy;
using UltimaRX.Packets;
using UltimaRX.Proxy.InjectionApi;
using UltimaRX.Packets.Parsers;
using UltimaRX.Gumps;
using static UltimaRX.Proxy.InjectionApi.Injection;
using static Scripts;

void Pickup(Item item)
{
    MoveItem(item, Me.BackPack);
}

void MoveItems(IEnumerable<Item> items, Item targetContainer)
{
    foreach (var item in items)
    {
        MoveItem(item, targetContainer);
    }
}

void MoveItems(IEnumerable<Item> items, ushort amount, Item targetContainer)
{
    foreach (var item in items)
    {
        if (amount == 0)
            break;

        if (item.Amount <= amount)
        {
            MoveItem(item, targetContainer);
            amount -= item.Amount;
        }
        else
        {
            MoveItem(item, amount, targetContainer);
            amount = 0;
        }
    }
}
