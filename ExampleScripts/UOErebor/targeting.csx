#load "Specs.csx"

using System.Collections.Generic;
using System.Linq;
using Infusion.Packets;

public static class Targeting
{
    private static HashSet<uint> alreadyTargeted = new HashSet<uint>();
    private static uint? lastTarget;
    private static uint? previousTarget;
    
    private static MobileSpec IgnoredTargets = new[] { Specs.Satan };    
    
    public static void TargetNext()
    {
        var potentialTargets = UO.Mobiles.MaxDistance(20)
            .NotMatching(IgnoredTargets)
            .Where(i => i.Notoriety == Notoriety.Murderer)
            .OrderByDistance();
        if (!potentialTargets.Any())
        {
            UO.ClientPrint("No potential target");
            return;
        }
        
        var target = potentialTargets.FirstOrDefault(i => !alreadyTargeted.Contains(i.Id));
        
        if (target != null)
        {
            if (string.IsNullOrEmpty(target.Name))
            {
                UO.ClientPrint("******", "targeting", target.Id, target.Type, SpeechType.Speech, Colors.Green, log: false);
                UO.Click(target);
            }
            else
                UO.ClientPrint($"*** {target.Name} ***", "targeting", target.Id, target.Type, SpeechType.Speech, Colors.Green, log: false);
                
            UO.ClientPrint($"Target distance: {target.GetDistance(UO.Me.Location)}", log: false);
            
            lastTarget = target.Id;
            alreadyTargeted.Add(target.Id);
        }
        else
        {
            alreadyTargeted.Clear();
            TargetNext();
        }
    }
    
    public static void LastTarget()
    {
        if (!lastTarget.HasValue)
            return;
        var target = UO.Mobiles[lastTarget.Value];
        if (target == null || target.GetDistance(UO.Me.Location) > 50)
            return;
    
        if (previousTarget != lastTarget)
        {
            previousTarget = lastTarget;
            if (UO.TryAttack(target) != AttackResult.Accepted)
                UO.ClientPrint($"Cannot attack {target}"); 
            
            UO.Wait(50);
            UO.WarModeOff();
            UO.Wait(50);
            alreadyTargeted.Clear();
            alreadyTargeted.Add(target.Id);
        }
        UO.Target(target);
    }
}

UO.RegisterCommand("targetnext", Targeting.TargetNext);
UO.RegisterCommand("lasttarget", Targeting.LastTarget);
