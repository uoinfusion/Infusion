using System;
using Infusion.LegacyApi;

class Program
{
    public static Player Me = UO.Me;
    public static bool IsBusy = false;

    public static void Main(string[] args)
    {
        while (!Me.IsDead)
        {
            {
                IsBusy = true;
                while (IsBusy)
                {
                    var _MageMax = Me.Skills.Magery.Cap;
                    var _CurMage = Me.Skills.Magery.Current;

                    if (Me.CurrentMana < 25)
                        Medit();

                    if (_CurMage >= 0 && _CurMage < 34)
                    {
                        UO.CastSpell(Infusion.Spell.Bless);
                        UO.WaitForTarget();
                        UO.Target(Me.PlayerId);
                    }
                    else if (_CurMage >= 34 && _CurMage < 45)
                    {
                        UO.CastSpell(Infusion.Spell.GreaterHeal);
                        UO.WaitForTarget();
                        UO.Target(Me.PlayerId);
                    }
                    else if (_CurMage >= 45 && _CurMage < 67)
                    {
                        UO.CastSpell(Infusion.Spell.DispelField);
                        UO.WaitForTarget();
                        UO.Target(Me.PlayerId);
                    }
                    else if (_CurMage >= 67 && _CurMage < 82)
                    {
                        UO.CastSpell(Infusion.Spell.Invisibility);
                        UO.WaitForTarget();
                        UO.Target(Me.PlayerId);
                    }
                    else if (_CurMage >= 82 && _CurMage < 95)
                    {
                        UO.CastSpell(Infusion.Spell.MassDispel);
                        UO.WaitForTarget();
                        UO.Target(Me.PlayerId);
                    }
                    else if (_CurMage >= 95 && _CurMage < 120)
                    {
                        if (Me.CurrentMana < 40)
                            Medit();
                        UO.CastSpell(Infusion.Spell.Earthquake);
                        UO.Wait(1500);
                    }
                    else
                    {
                        IsBusy = false;
                    }
                }
                break;
            }
        }
    }
    public static void Medit()
    {
        do
        {
            UO.ClearTargetObject();
            UO.UseSkill(Infusion.Skill.Meditation);
            UO.Wait(500);
        } while (Me.CurrentMana < Me.MaxMana);
    }
}
