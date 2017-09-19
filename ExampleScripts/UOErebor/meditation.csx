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
        UO.Log($"Meditating, current mana is {UO.Me.CurrentMana}, maximum mana is {UO.Me.MaxMana}");
        while (UO.Me.CurrentMana < UO.Me.MaxMana)
        {
            UO.UseSkill(Skill.Meditation);
            UO.Wait(2000);
        }
    }
}