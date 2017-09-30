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
                .Where(i => i.Id != UO.Me.PlayerId && i.Notoriety == Notoriety.Murderer && !i.CanModifyName && !Targeting.Ignored.Contains(i. Id))
                .OrderByDistance();
                
    public static TargetingMode Pvp = () =>
        UO.Mobiles.MaxDistance(20)
                .Matching(Specs.Player)
                .Where(i => i.Id != UO.Me.PlayerId && i.Notoriety == Notoriety.Murderer && !Targeting.Ignored.Contains(i. Id))
                .OrderByDistance();

    public static TargetingMode PvpFriend = () =>
        UO.Mobiles.MaxDistance(20)
                .Matching(Specs.Player)
                .Where(i => i.Id != UO.Me.PlayerId && !Targeting.Ignored.Contains(i. Id))
                .OrderByDistance();
}

public static class Targeting
{
    private static Stack<ObjectId> alreadyTargeted = new Stack<ObjectId>();
    private static ObjectId? selectedTarget;
    private static ObjectId? currentTarget;
    private static ObjectId? currentSelection;

    public static TargetingMode Mode { get; set; } = TargetingModes.Pvm;

    public static bool AutoTurnOffWarMode { get; set; } = false;

    public static IMobileLookup Ignored { get; set; } = Pets.MyPets;

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
                .Where(i => i.Notoriety == Notoriety.Murderer && !i.CanModifyName && !Ignored.Contains(i. Id))
                .OrderByDistance();
        }
    }

    public static void TargetNext()
    {
        PrintAlreadyTargeted();

        var potentialTargets = GetTargets(); 
        if (!potentialTargets.Any())
        {
            UO.ClientPrint("No potential target");
            return;
        }
        
        var target = potentialTargets.FirstOrDefault(i =>
            // excluding mobiles already target
            !alreadyTargeted.Contains(i.Id) &&
            (!selectedTarget.HasValue || selectedTarget.Value != i.Id));
        
        if (target != null)
        {
            SelectTarget(target);
        
            alreadyTargeted.Push(target.Id);
        }
        else
        {
            if (potentialTargets.Count() == 1 && selectedTarget == potentialTargets.First().Id)
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
        UO.Log(alreadyTargeted
            .Select(x => x.ToString())
            .Aggregate("Targeted: ", (l, r) => l + ", " + r));
    }
    
    public static void TargetPrev()
    {
        Mobile target = null;

        PrintAlreadyTargeted();

        if (alreadyTargeted.Any())
        {
            if (selectedTarget.HasValue && selectedTarget.Value == alreadyTargeted.Peek())
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
            
        UO.ClientPrint($"Target distance: {target.GetDistance(UO.Me.Location)}", log: false);
        
        selectedTarget = target.Id;
    }
    
    public static void TargetLast()
    {
        if (!selectedTarget.HasValue)
            return;
        var target = UO.Mobiles[selectedTarget.Value];
        if (target == null)
            return;
    
        if (currentTarget != selectedTarget)
        {
            // When we are targeting a target for the first time:
            currentTarget = selectedTarget;

            // We want to force the game client to display a status bar 
            // of the new target. Attacking the new target seems
            // to be the only way how to do it on 3.0.6m.
            if (UO.TryAttack(target) != AttackResult.Accepted)
                UO.ClientPrint($"Cannot attack {target}"); 

            // For characters like mage, necro or cleric, it is unwanted
            // to turn on war mode just by targeting. Turn it off
            // automatically enalbed.            
            if (AutoTurnOffWarMode)
                UO.WarModeOff();

            // Restart targeting by clearing list of already targeted
            // items. We have to add current target, so ',targetnext'
            // skips current target and really moves to the next one.
            alreadyTargeted.Clear();
            alreadyTargeted.Push(target.Id);
        }
        
        UO.Target(target);
    }
}

UO.RegisterCommand("target-next", Targeting.TargetNext);
UO.RegisterCommand("target-prev", Targeting.TargetPrev);
UO.RegisterCommand("target-last", Targeting.TargetLast);
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