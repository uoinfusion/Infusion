#load "Specs.csx"

using System;
using System.Collections.Generic;
using System.Linq;
using Infusion.Packets;

public static class Targeting
{
    private static Stack<ObjectId> alreadyTargeted = new Stack<ObjectId>();
    private static ObjectId? selectedTarget;
    private static ObjectId? currentTarget;
    private static ObjectId? currentSelection;
    
    public static MobileSpec IgnoredTargets = new[] { Specs.Satan };    
    
    public static bool AutoTurnOffWarMode { get; set; } = false;

    private static IEnumerable<Mobile> GetTargets() =>
        UO.Mobiles.MaxDistance(20)
                .NotMatching(IgnoredTargets)
                .Where(i => i.Notoriety == Notoriety.Murderer)
                .OrderByDistance();

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

UO.RegisterCommand("targetnext", Targeting.TargetNext);
UO.RegisterCommand("targetprev", Targeting.TargetPrev);
UO.RegisterCommand("lasttarget", Targeting.TargetLast);
