using System;

public static class Meditation
{
    public static TimeSpan MeditationAttemptInterval { get; } = TimeSpan.FromSeconds(2);

    public static void Meditate(int manaMinimum)
    {
        UO.Log($"Meditating at least to {manaMinimum}, current mana is {UO.Me.CurrentMana}");
    
        UO.WarModeOff();
        UO.Wait(100);
    
        while (UO.Me.CurrentMana < manaMinimum)
        {
            UO.UseSkill(Skill.Meditation);
            UO.Wait(MeditationAttemptInterval);
        }
    }
    
    public static void Meditate()
    {
        UO.WarModeOff();

        UO.Log($"Meditating to maximum mana {UO.Me.MaxMana}, current mana is {UO.Me.CurrentMana}");
        while (UO.Me.CurrentMana < UO.Me.MaxMana)
        {
            UO.UseSkill(Skill.Meditation);
            UO.Wait(MeditationAttemptInterval);
        }
    }
    
    public static void Meditate(Func<bool> abortConditionFunc)
    {
        UO.WarModeOff();
        DateTime lastMeditionAttempt = DateTime.MinValue;

        UO.Log($"Meditating to maximum mana {UO.Me.MaxMana}, current mana is {UO.Me.CurrentMana}");
        while (UO.Me.CurrentMana < UO.Me.MaxMana)
        {
            if (abortConditionFunc != null && abortConditionFunc())
            {
                UO.Log("Meditation aborted");
                return;
            }

            var now = DateTime.UtcNow;
            
            if (now - lastMeditionAttempt > MeditationAttemptInterval)
            {
                UO.UseSkill(Skill.Meditation);
                lastMeditionAttempt = now;
            }
            
            UO.Wait(100);
        }
    }
}