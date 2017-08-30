#load "items.csx"

using System.Linq;

public static class Selling
{
    public static void SellFromBank(string itemName, int amountToSell)
    {
        ushort sellBatchAmount = 250;
    
        UO.ClientPrint("What item to sell?");
        var itemToSell = UO.AskForItem();
        if (itemToSell != null)
        {
            var itemToSellColor = itemToSell.Color;
            var itemToSellType = itemToSell.Type;
            Item itemToSellContainer = null;
            if (itemToSell.ContainerId.HasValue)
                itemToSellContainer = UO.Items[itemToSell.ContainerId.Value];
            else
                UO.ClientPrint("Cannot sell items that are not in container");
        
            var itemsToSell = UO.Items.OfType(itemToSell.Type).OfColor(itemToSell.Color).InContainer(itemToSellContainer);
        
            while (amountToSell > 0 && itemsToSell != null && itemsToSell.Any() && itemsToSell.Sum(i => i.Amount) >= sellBatchAmount)
            {
                UO.ClientPrint($"selling {sellBatchAmount} of {itemName}, {amountToSell} remaining");
            
                Items.MoveItems(itemsToSell, sellBatchAmount, UO.Me.BackPack);
            
                UO.Say("sell");
                UO.WaitForGump();
                UO.SelectGumpButton($"{sellBatchAmount} {itemName}", Infusion.Gumps.GumpLabelPosition.After);
                UO.WaitForGump();
                UO.TriggerGump((GumpControlId)1);
                UO.WaitForGump();
                UO.CloseGump();
                UO.Wait(1000);
                amountToSell -= sellBatchAmount;
            }
        }
        else
            UO.ClientPrint("sell cancelled");
    }
}
