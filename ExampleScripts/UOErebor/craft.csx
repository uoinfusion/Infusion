#load "TwoStateAbility.csx"

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