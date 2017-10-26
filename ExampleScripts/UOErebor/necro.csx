#load "TwoStateAbility.csx"

using System;

public static class Necro
{
    public static TwoStateAbility DarkPower { get; } = new TwoStateAbility(".darkpower",
        "Nyni ti muze byt navracena spotrebovana mana.", "Jsi zpatky v normalnim stavu.");
    public static TwoStateAbility StandCast { get; } = new TwoStateAbility(".standcast",
        "Nyni budes kouzlit s vetsim usilim.", "Tve usili polevilo.");
}
