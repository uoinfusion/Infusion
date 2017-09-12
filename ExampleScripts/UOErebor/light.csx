#load "Specs.csx"

using System;
using Infusion.Commands;

public delegate void LightSourceDelegate();

public static class LightSources
{
    public static readonly LightSourceDelegate Torch = () =>
        MakeLightByUse(Specs.Torch);
    
    public static readonly LightSourceDelegate Potion = () =>
        MakeLightByUse(Specs.NightsighPoition);
    
    private static void MakeLightByUse(ItemSpec spec)
    {
        var item =
            UO.Items.InContainer(UO.Me.BackPack).Matching(spec).FirstOrDefault();
        if (item != null)
        {
            UO.Use(item);
            UO.Wait(1000);
        }
        else
            UO.Alert($"Cannot find {Specs.TranslateToName(spec) ?? "any source of light"}.");
    }
}

public static class Light
{
    private static GameJournal lightCheckJournal = UO.CreateJournal();

    public static LightSourceDelegate PreferredLightSource { get; set; } = LightSources.Torch;

    public static void MakeLight()
    {
        if (PreferredLightSource == null)
            throw new CommandInvocationException(
                "Light.PreferedLightSource is null. Please run this code " +
                "to set a light source from LightSources class: " +
                "'Light.PreferredLightSource = LightSources.Torch;'. " + 
                "Instead Torch you can choose any other light source from " +
                "LightSources class.");
    
        PreferredLightSource();
    }

    public static void Check()
    {
        if (lightCheckJournal.Contains("Je spatne videt"))
        {
            lightCheckJournal.Delete();
            MakeLight();
        }
    }
}
