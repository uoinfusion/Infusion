#load "TwoStateAbility.csx"
#load "blacksmith.csx"
#load "tinkering.csx"
#load "carpentry.csx"
#load "bowcraft.csx"
#load "tailoring.csx"
#load "doors.csx"
#load "looting.csx"
#load "hpnotify.csx"
#load "hiding.csx"
#load "light.csx"
#load "magery.csx"
#load "party.csx"
#load "questarrow.csx"
#load "title.csx"
#load "healing.csx"


public static class Craft
{
    public static TwoStateAbility Vigour { get; } = new TwoStateAbility(".vigour",
        "Nyni jsi schopen nalezt lepsi materialy.", "Jsi zpatky v normalnim stavu.");
        
    public static void VigourOn()
    {
        Vigour.TurnOn();
    }

    public static void VigourOff()
    {
        Vigour.TurnOff();
    }
}