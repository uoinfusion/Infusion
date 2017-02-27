#load "ItemTypes.csx"
#load "Scripts.csx"

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UltimaRX.Proxy;
using UltimaRX.Packets;
using UltimaRX.Proxy.InjectionApi;
using UltimaRX.Packets.Parsers;
using UltimaRX.Gumps;
using static UltimaRX.Proxy.InjectionApi.Injection;
using static Scripts;

ModelId[] ignoredLoot = ItemTypes.Torsos;

const uint lootContainerId = 0x4005EEE6;
const uint unknownLootContainerId = 0x4002ADF2;

HashSet<ModelId> wellKnownLoot = new HashSet<ModelId>
{
    (ModelId)0x186F, // vanocni banka
    (ModelId)0x0f8f, // VA
    (ModelId)0x0f19, // Sapphire
    (ModelId)0x0e76, // Item
    (ModelId)0x0f8b, // Pumice
    (ModelId)0x0f13, // Ruby
    (ModelId)0x0f26, // Diamond
    (ModelId)0x0f78, // BatWings
    (ModelId)0x0f8c, // SA
    (ModelId)0x0f7b, // BM
    (ModelId)0x0f7a, // BP
    (ModelId)0x0f84, // Garlic
    (ModelId)0x0f86, // Mandrake
    (ModelId)0x0f88, // Nightshade
    (ModelId)0x0f85, // Ginseng
    (ModelId)0x0f89, // Obsidian
    (ModelId)0x0f81, // Feritle Dirt
    (ModelId)0x0f91, // Wirth's Hearth
    (ModelId)0x0f87, // Eyes of Newt
    (ModelId)0x0f8d, // SS
    (ModelId)0x0F82, // DragonsBlood
    (ModelId)0x0F7D, // Novej
    (ModelId)0x1bfb, // sipka
    (ModelId)0x1078, // kuze
    (ModelId)0x0f80, // kosti
    (ModelId)0x0f11, // drahokam
    (ModelId)0x0f10, // drahokam
    (ModelId)0x0f15, // drahokam
    (ModelId)0x0f8a, // ironpig
    (ModelId)0x0f0f, // drahokam
    (ModelId)0x0f25, // drahokam
    (ModelId)0x0f16, // drahokam
    (ModelId)0x0f18, // drahokam
    (ModelId)0x1bd1, // peri
    (ModelId)0x0F7E, // kosti normal
    (ModelId)0x0F83, // prasivky
    (ModelId)0x0F8E, // Nox crystal
    (ModelId)0x14EB, // Tajemna mapa
    (ModelId)0x1726, // kraslice
    (ModelId)0x0C77, // mrkev
    (ModelId)0x0C6D, // cibule
    (ModelId)0x186F, // vanocni banka
    (ModelId)0x0eed, // gold
    (ModelId)0x186F, // vanocni banka
};

void RipAndLootNearest()
{
    var corpse = Items.OfType(ItemTypes.Corpse).MaxDistance(Me.Location, 3).OrderByDistance(Me.Location).First();

    if (corpse != null)
    {
        Rip(corpse);
        Loot(corpse);
        Ignore(corpse);
    }
}

void LootNearest()
{
    var corpse = Items.OfType(ItemTypes.Corpse).MaxDistance(Me.Location, 3).OrderByDistance(Me.Location).First();

    if (corpse != null)
    {
        Loot(corpse);
        Ignore(corpse);
    }
}

void Loot()
{
    var container = ItemInfo();
    if (container != null)
    {
        Loot(container);
    }
    else
        Log("no container for loot");
}

void Loot(Item container)
{
    var targetLootContainer = Items[lootContainerId] ?? Me.BackPack;
    var targetUnknownLootContainer = Items[unknownLootContainerId] ?? Me.BackPack;

    foreach (var itemToPickup in Items.InContainer(container).ToArray())
    {
        Log($"Looting type {itemToPickup.Type}");
        if (!ignoredLoot.Contains(itemToPickup.Type))
        { 
            if (wellKnownLoot.Contains(itemToPickup.Type))
                MoveItem(itemToPickup, targetLootContainer);
            else
                MoveItem(itemToPickup, targetUnknownLootContainer);

            Wait(100);
            if (InJournal("Ne tak rychle!"))
            {
                DeleteJournal();
                Wait(4000);
            }
        }
        else 
            Ignore(itemToPickup);
    }

    Log("Looting finished");
}

void Rip(Item container)
{
    var itemInHand = Items.OnLayer(Layer.OneHandedWeapon).First() ?? Items.OnLayer(Layer.TwoHandedWeapon).First();
    UseType(ItemTypes.Knives);
    WaitForTarget();
    Target(container);
    WaitForJournal("Rozrezal jsi mrtvolu.");

    if (itemInHand != null)
    {
        DragItem(itemInHand);
        Wait(100);
        Wear(itemInHand, Layer.OneHandedWeapon);
        Wait(100);
    }
}

Injection.CommandHandler.RegisterCommand(new Command("ripandloot", RipAndLootNearest));
Injection.CommandHandler.RegisterCommand(new Command("loot", LootNearest));
