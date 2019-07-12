#load "Specs.csx"
#load "colors.csx"

using System;
using System.Collections.Generic;

public enum LeverPositions
{
    Left = 1,
    Up = 2,
    Right = 3
}

public static class Triggers
{
    public static void UseNearest()
    {
        var lever = UO.Items.Matching(Specs.Switches).OnGround().OrderByDistance().FirstOrDefault();
        if (lever == null)
        {
            UO.ClientPrint("No trigger found", UO.Me, Colors.Red);
            return;
        }
        
        UO.Use(lever);
    }
    
    public static void SwitchNearestToCommand(string parameters)
    {
        if (!Enum.TryParse<LeverPositions>(parameters, true, out LeverPositions target))
        {
            UO.Console.Error("Requires target lever position: left, up, right");
            return;
        }

        SwitchNearestTo(target);
    }
    
    public static void SwitchNearestTo(LeverPositions target)
    {
        UseNearest();
        UO.WaitForGump();
        
        var leverPositionProcessor = new LeverGumpProcessor();
        var parser = new GumpParser(leverPositionProcessor);
        parser.Parse(UO.CurrentGump);
        
        if (!leverPositionProcessor.CurrentPosition.HasValue)
        {
            UO.ClientPrint("Cannot see lever gump", UO.Me, Colors.Red);
            return;
        }
        
        LeverPositions current = leverPositionProcessor.CurrentPosition.Value;

        int pullCount = target - current;
        while (pullCount != 0)
        {
            if (pullCount > 0)
            {
                // pull to the left
                UO.TriggerGump(2);
                pullCount--;
            }
            else
            {
                // pull to the right
                UO.TriggerGump(1);
                pullCount++;
            }
            UO.WaitForGump();
        }
        
        UO.CloseGump();
    }
    
    private class LeverGumpProcessor : IProcessTilePicHue
    {
        public LeverPositions? CurrentPosition { get; private set; }
    
        public void OnTilePicHue(int x, int y, uint itemId, int hue)
        {
            if (x == 80)
            {
                switch (itemId)
                {
                    case 4236:
                        CurrentPosition = LeverPositions.Left;
                        break;
                    case 4237:
                        CurrentPosition = LeverPositions.Up;
                        break;
                    case 4238:
                        CurrentPosition = LeverPositions.Right;
                        break;
                }
            }
        }
    }
}

UO.RegisterCommand("trigger-usenearest", Triggers.UseNearest);
UO.RegisterCommand("lever-switchnearest", Triggers.SwitchNearestToCommand);