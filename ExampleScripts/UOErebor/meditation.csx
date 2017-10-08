public static class Meditation
{
    public static void Meditate(int manaMinimum)
    {
        UO.Log($"Meditating at least to {manaMinimum}, current mana is {UO.Me.CurrentMana}");
    
        UO.WarModeOff();
        UO.Wait(100);
    
        while (UO.Me.CurrentMana < manaMinimum)
        {
            UO.UseSkill(Skill.Meditation);
            UO.Wait(2000);
        }
    }
    
    public static void Meditate()
    {
        UO.WarModeOff();

        UO.Log($"Meditating to maximum mana {UO.Me.MaxMana}, current mana is {UO.Me.CurrentMana}");
        while (UO.Me.CurrentMana < UO.Me.MaxMana)
        {
            UO.UseSkill(Skill.Meditation);
            UO.Wait(2000);
        }
    }
}