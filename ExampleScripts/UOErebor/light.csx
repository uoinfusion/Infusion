#load "Specs.csx"

using System;
using Infusion.Commands;

public delegate void LightSource();

public static class LightSources
{
    public static readonly LightSource Torch = () =>
        MakeLightByUse(Specs.Torch);
    
    public static readonly LightSource Potion = () =>
        MakeLightByUse(Specs.NightsighPoition);
        
    public static readonly LightSource Spell = () =>
    {
        UO.CastSpell(Infusion.Spell.NightSight);
        UO.WaitForTarget();
        UO.Target(UO.Me);
        UO.Wait(2000);
    };
    
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
            UO.ClientPrint($"Cannot find {Specs.TranslateToName(spec) ?? "any source of light"}.", UO.Me);
    }
}

public static class Light
{
    private static SpeechJournal lightCheckJournal = UO.CreateSpeechJournal();

    public static LightSource PreferredLightSource { get; set; } = LightSources.Torch;

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

    public static bool Check()
    {
        if (IsLightNeeded)
        {
            MakeLight();
            lightCheckJournal.Delete();
            return true;
        }
        
        return false;
    }
    
    public static bool IsLightNeeded
    {
        get
        {
            bool result = lightCheckJournal.Contains("Je spatne videt");
            lightCheckJournal.Delete();
            
            return result;
        }
    }
}
