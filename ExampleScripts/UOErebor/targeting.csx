#load "colors.csx"
#load "Specs.csx"
#load "common.csx"
#load "pets.csx"

using System;
using System.Collections.Generic;
using System.Linq;
using Infusion.Packets;

public delegate IEnumerable<Mobile> TargetingMode();

public static class TargetingModes
{
    public static TargetingMode Pvm = () =>
        UO.Mobiles.MaxDistance(20)
                // considering just murderers (red karma) and mobiles that are
                // not player's pets/summons - player can change name of her/his
                // pets/summons.
                .Where(i => i.Id != UO.Me.PlayerId && i.Notoriety == Notoriety.Murderer
                    && !i.CanRename && !Targeting.Ignored.Contains(i. Id) && (Targeting.IgnoredSpec == null || !Targeting.IgnoredSpec.Matches(i)))
                .OrderByDistance();
                
    public static TargetingMode Pvp = () =>
        UO.Mobiles.MaxDistance(20)
                .Matching(Specs.Player)
                .Where(i => i.Id != UO.Me.PlayerId && (i.Notoriety == Notoriety.Murderer 
                    || ((i.Notoriety == Notoriety.Grey ||  i.Notoriety == Notoriety.Criminal) && Specs.Player.Matches(i)))
                    && !Targeting.Ignored.Contains(i. Id)&& (Targeting.IgnoredSpec == null || !Targeting.IgnoredSpec.Matches(i)))
                .OrderByDistance();

    public static TargetingMode PvpFriend = () =>
        UO.Mobiles.MaxDistance(20)
                .Matching(Specs.Player)
                .Where(i => i.Id != UO.Me.PlayerId && !Targeting.Ignored.Contains(i. Id) 
                    && (Targeting.IgnoredSpec == null || !Targeting.IgnoredSpec.Matches(i)))
                .OrderByDistance();
}

public static class Targeting
{
    private static Stack<ObjectId> alreadyTargeted = new Stack<ObjectId>();
    public static ObjectId? SelectedTarget { get; private set; }
    public static ScriptTrace Trace { get; } = UO.Trace.Create();

    public static ObjectId? CurrentTarget { get; private set; }

    public static TargetingMode Mode { get; set; } = TargetingModes.Pvm;

    public static IMobileLookup Ignored { get; set; } = Pets.MyPets;
    
    public static MobileSpec IgnoredSpec { get; set; } = null;
    
    public static event Action<ObjectId> TargetingLast;
    public static event Action<ObjectId> AttackingLast;

    private static IEnumerable<Mobile> GetTargets()
    {
        if (Mode != null)
            return Mode();
        else
        {
            return UO.Mobiles.MaxDistance(20)
                // considering just murderers (red karma) and mobiles that are
                // not player's pets/summons - player can change name of her/his
                // pets/summons.
                .Where(i => i.Notoriety == Notoriety.Murderer && !i.CanRename && !Ignored.Contains(i. Id) 
                    && (IgnoredSpec == null || !IgnoredSpec.Matches(i)))
                .OrderByDistance();
        }
    }

    public static void TargetNext()
    {
        PrintAlreadyTargeted();

        var potentialTargets = GetTargets(); 
        if (!potentialTargets.Any())
        {
            UO.ClientPrint("No potential target", "targeting", UO.Me);
            return;
        }
        
        var target = potentialTargets.FirstOrDefault(i =>
            // excluding mobiles already target
            !alreadyTargeted.Contains(i.Id) &&
            (!SelectedTarget.HasValue || SelectedTarget.Value != i.Id));
        
        if (target != null)
        {
            SelectTarget(target);
        
            alreadyTargeted.Push(target.Id);
        }
        else
        {
            if (potentialTargets.Count() == 1 && SelectedTarget == potentialTargets.First().Id)
                SelectTarget(potentialTargets.First());
            else
            {
                // start the targeting from begining 
                // by clearing already targeted mobiles
                alreadyTargeted.Clear();
                
                // and trying to target next again
                TargetNext();
            }
        }
    }
    
    public static void PrintAlreadyTargeted()
    {
        if (Trace.Enabled)
        {
            Trace.Log(alreadyTargeted
                .Select(x => x.ToString())
                .Aggregate("Targeted: ", (l, r) => l + ", " + r));
        }
    }
    
    public static void TargetPrev()
    {
        Mobile target = null;

        PrintAlreadyTargeted();

        if (alreadyTargeted.Any())
        {
            if (SelectedTarget.HasValue && SelectedTarget.Value == alreadyTargeted.Peek())
                alreadyTargeted.Pop();
    
            while (alreadyTargeted.Any() && (target = UO.Mobiles[alreadyTargeted.Pop()]) == null)
                ;
        }

        if (target == null)
        {
            Mobile lastTarget = null;
            var potentialTargets = GetTargets().ToArray();
            if (!potentialTargets.Any())
            {
                UO.ClientPrint("No potential previous target");
                return;
            }
            
            foreach (var t in potentialTargets)
            {
                alreadyTargeted.Push(t.Id);
                lastTarget = t;
            }

            if (alreadyTargeted.Count() == 1)
                SelectTarget(potentialTargets.First());
            else
                TargetPrev();
        }
        else
        {
            SelectTarget(target);
        }
    }
    
    private static void SelectTarget(Mobile target)
    {
        if (string.IsNullOrEmpty(target.Name))
        {
            UO.ClientPrint("******", "targeting", target.Id, target.Type, SpeechType.Speech, Colors.Green, log: false);
            UO.Click(target);
        }
        else
            UO.ClientPrint($"*** {target.Name} ***", "targeting", target.Id, target.Type, SpeechType.Speech, Colors.Green, log: false);
        
        UO.Client.AllowAttack(target.Id);
        UO.ClientPrint($"Target distance: {target.GetDistance(UO.Me.Location)}", log: false);
        
        SelectedTarget = target.Id;
    }
    
    public static void AttackLast()
    {
        if (!SelectedTarget.HasValue)
        {
            UO.ClientPrint("No selected target", "targeting", UO.Me);
            return;
        }
        
        var target = UO.Mobiles[SelectedTarget.Value];
        if (target == null)
        {
            UO.ClientPrint("Cannot see target", "targeting", UO.Me);
            return;
        }

        var attackResult = UO.TryAttack(target); 
        if (attackResult != AttackResult.Accepted)
        {
            UO.ClientPrint($"Cannot attack {target.Name ?? "last target"}");
            if (Trace.Enabled)
                Trace.Log($"AttackResult is {attackResult}, target: {target}");
        }            

        alreadyTargeted.Clear();
        alreadyTargeted.Push(target.Id);
    }
    
    public static void TargetLast()
    {
        if (!SelectedTarget.HasValue)
        {
            UO.ClientPrint("No selected target", "targeting", UO.Me);
            return;
        }
        
        var target = UO.Mobiles[SelectedTarget.Value];
        if (target == null)
        {
            UO.ClientPrint("Cannot see target", "targeting", UO.Me);
            return;
        }
    
        if (CurrentTarget != SelectedTarget)
        {
            // When we are targeting a target for the first time:
            CurrentTarget = SelectedTarget;

            // Restart targeting by clearing list of already targeted
            // items. We have to add current target, so ',targetnext'
            // skips current target and really moves to the next one.
            alreadyTargeted.Clear();
            alreadyTargeted.Push(target.Id);
        }

        TargetingLast.Invoke(target.Id);
        UO.Target(target);
    }
    
    public static void SelectTarget()
    {
        var mobile = UO.AskForMobile();
        if (mobile == null)
        {
            UO.Log("Targeting canceled");
            return;
        }
        
        SelectedTarget = mobile.Id;
        CurrentTarget = mobile.Id;
        alreadyTargeted.Clear();
        alreadyTargeted.Push(mobile.Id);
    }
}

UO.RegisterCommand("target-select", Targeting.SelectTarget);
UO.RegisterCommand("target-next", Targeting.TargetNext);
UO.RegisterCommand("target-prev", Targeting.TargetPrev);
UO.RegisterCommand("target-last", Targeting.TargetLast);
UO.RegisterCommand("target-attack", Targeting.AttackLast);
UO.RegisterCommand("target-pvm", () =>
{
    Targeting.Mode = TargetingModes.Pvm;
    UO.ClientPrint("Pvm targeting");
});
UO.RegisterCommand("target-pvp", () =>
{
    Targeting.Mode = TargetingModes.Pvp;
    UO.ClientPrint("Pvp targeting");
});
UO.RegisterCommand("target-pvpfriend", () => 
{
    Targeting.Mode = TargetingModes.PvpFriend;
    UO.ClientPrint("Pvp with friends targeting");
});
