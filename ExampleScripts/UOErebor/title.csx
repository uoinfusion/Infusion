#load "Specs.csx"

using System;
using System.Linq;

public static class Title
{
    public static TimeSpan RefreshTime { get; set; } = TimeSpan.FromSeconds(2);
    public static Func<string> Text = () =>
        $"{UO.Me.Name} - Weight: {UO.Me.Weight}, GP: {UO.Me.Gold}, Armor: {UO.Me.Armor}, " +
        $"Int: {UO.Me.Intelligence}, Dex: {UO.Me.Dexterity}, Str: {UO.Me.Strength}, " +
        $"Food: {UO.Items.InContainer(UO.Me.BackPack).Matching(Specs.Food).Sum(x => x.Amount):###}, ";
        
    public static void Run()
    {
        while (true)
        {
            var text = Text();
            UO.ClientWindow.SetTitle(text);
            UO.Wait(RefreshTime);
        }
    }
    
    public static void Enable()
    {
        UO.CommandHandler.Invoke(",title");
    }
    
    public static void Disable()
    {
        UO.CommandHandler.Terminate("title");
    }
}

UO.RegisterBackgroundCommand("title", Title.Run);
UO.RegisterCommand("title-enable", Title.Enable);
UO.RegisterCommand("title-disable", Title.Disable);