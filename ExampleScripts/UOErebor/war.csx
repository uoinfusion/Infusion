#load "TwoStateAbility.csx"

using System;

public static class War
{
    private static EventJournal eventJournal = UO.CreateEventJournal();

    public static TwoStateAbility Berserker { get; } = new TwoStateAbility(".berserker",
        "Nyni jsi schopen zasadit kriticky uder.", "Jsi zpatky v normalnim stavu.",
        Array.Empty<string>(),
        new StateIndicator(0x80000000, 0x99000000, 2295, 0x20, 0));
        
}

