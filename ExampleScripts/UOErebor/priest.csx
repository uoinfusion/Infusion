#load "TwoStateAbility.csx"

using System;

public static class Priest
{
    private static EventJournal eventJournal = UO.CreateEventJournal();

    public static TwoStateAbility Enlightment { get; } = new TwoStateAbility(".enlightment",
        "Nyni jsi schopen rychleji lecit bandazemi.", "Jsi zpatky v normalnim stavu.");
        
}

UO.RegisterCommand("selfress", () => UO.Say(".resurrection"));
