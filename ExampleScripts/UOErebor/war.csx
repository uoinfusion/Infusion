#load "TwoStateAbility.csx"

using System;

public static class War
{
    private static EventJournal eventJournal = UO.CreateEventJournal();

    public static TwoStateAbility Berserker { get; } = new TwoStateAbility(".berserker",
        "Nyni jsi schopen zasadit kriticky uder.", "Jsi zpatky v normalnim stavu.");
        
}

