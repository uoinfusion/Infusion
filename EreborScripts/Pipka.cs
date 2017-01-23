using System;
using System.ComponentModel;
using System.Linq;
using UltimaRX.Gumps;
using UltimaRX.Packets;
using UltimaRX.Proxy.InjectionApi;
using static UltimaRX.Proxy.InjectionApi.Injection;
using static Scripts;

public static class Pipka
{
    public const int UnloadContainerId = 0x40057064;

    public static readonly ModelId[] TypesToUnload =
        new[] {ItemTypes.Feathers, ItemTypes.Logs, ItemTypes.Furs, ItemTypes.PileOfHides}
            .Concat(ItemTypes.RawFood)
            .Concat(ItemTypes.Fishes)
            .ToArray();

    public static readonly Action DolAmrothBank2Gate = Script.Create(() => Harvest("DolAmroth-bank2gate.map"));
    public static readonly Action DolAmrothGate2Bank = Script.Create(() => Harvest("DolAmroth-gate2bank.map"));
    public static readonly Action DolAmrothLumberjacking = Script.Create(() => Harvest("dolamroth-lumberjacking.map"));
    public static readonly Action DolAmrothLumberjacking2 = Script.Create(() => Harvest("dolamroth-lumberjacking2.map"));
    public static readonly Action DolAmrothKilling = Script.Create(() => Harvest("DolAmroth-killing.map"));
    public static readonly Action LinhirHome2DolAmroth = Script.Create(() => Harvest("Linhir2DolAmroth.map"));

    public static void DolAmroth()
    {
        Run(() =>
        {
            while (true)
            {
                Harvest("DolAmroth-bank2gate.map");
                Harvest("dolamroth-lumberjacking.map");
                Harvest("DolAmroth-gate2bank.map");
                ReloadInDolAmrothBank();
            }
        });
    }

    public static void ReloadInDolAmrothBank()
    {
        Log("Unloading to bank in Dol Amroth");

        WalkToBanker();
        Wait(1000);

        OpenBank();
        Wait(1000);

        UnloadToBank();
        Wait(1000);

        RefreshFromBank();

        WalkOutFromBank();
    }

    public static void RefreshFromBank()
    {
        Log("Reloading from bank");

        if (Me.BankBox == null)
        {
            Log("Bank not opened");
            return;
        }

        ReloadPotion(Me.BankBox, ItemTypes.NightSightKegColor);
        Eat();
        Reload(Me.BankBox, 10, ItemTypes.Food);
    }

    private static void WalkOutFromBank()
    {
        WalkTo(2244, 3204);
        WalkTo(2245, 3204);
        OpenNearestDoor();
        WalkTo(2249, 3204);
        WalkTo(2249, 3209);
        WalkTo(2251, 3209);
        OpenNearestDoor();
        WalkTo(2255, 3209);
    }

    private static void WalkToBanker()
    {
        WalkTo(2253, 3209);
        OpenNearestDoor();

        WalkTo(2249, 3209);
        WalkTo(2249, 3204);
        WalkTo(2248, 3204);
        OpenNearestDoor();
        WalkTo(2244, 3204);
        WalkTo(2244, 3206);
    }

    private static void UnloadToBank()
    {
        var items = Items.OfType(TypesToUnload).InContainer(Me.BackPack);
        var unloadContainer = Items.Get(UnloadContainerId);
        MoveItems(items, unloadContainer);
    }

    private static void OpenBank()
    {
        Say("Hi");
        WaitForGump();
        GumpInfo();
        Wait(1000);

        SelectGumpButton("Bankovni sluzby", GumpLabelPosition.After);
        WaitForGump();
        GumpInfo();

        Wait(1000);
        SelectGumpButton("Otevrit banku.", GumpLabelPosition.After);
    }
}