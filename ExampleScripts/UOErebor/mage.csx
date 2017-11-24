#load "Specs.csx"
#load "TwoStateAbility.csx"

public static class Mage
{
    public static TwoStateAbility Concentration { get; } = new TwoStateAbility(".concentration",
        "Nyni jsi schopen kouzlit za mene many.", "Jsi zpatky v normalnim stavu.");
    public static TwoStateAbility StandCast { get; } = new TwoStateAbility(".standcast",
        "Nyni budes kouzlit s vetsim usilim.", "Tve usili polevilo.");
}
