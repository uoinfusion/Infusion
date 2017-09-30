#r "Infusion.Scripts.UOErebor.Extensions.dll"

#load "Specs.csx"
#load "common.csx"

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Infusion.Scripts.UOErebor.Extensions.StatusBars;

public static class Pets
{
    private static readonly Statuses statuses;

    public static MobileSpec PetsSpec = new[] { Specs.NecroSummons }; 

    static Pets()
    {
        UO.Events.MobileEnteredView += HandleMobileEnteredView;
        UO.Events.MobileLeftView += HandleMobileLeftView;

        statuses = new Statuses("Pets");
        UO.Events.HealthUpdated += HandleHealthUpdated;
        statuses.MobileTargeted += (sender, id) =>
        {
            var target = UO.Mobiles[id];
            if (target != null)
                UO.Target(target);
        };
    }
    
    private static void HandleHealthUpdated(object sender, CurrentHealthUpdatedArgs args)
    {
        if (statuses.Contains(args.UpdatedMobile))
        {
            statuses.Update(args.UpdatedMobile);
        }
        else if (args.UpdatedMobile.CanModifyName)
        {
            if (statuses.Count == 0)
            {
                foreach (var pet in MyPets)
                {
                    statuses.Add(pet, StatusBarType.Pet);
                }
            }

            statuses.Add(args.UpdatedMobile, StatusBarType.Pet);
        }
    }
    
    private static Dictionary<ObjectId, DateTime> statusRequests = new Dictionary<ObjectId, DateTime>();
   
    private static void HandleMobileEnteredView(object sender, Mobile mobile)
    {
        if (PetsSpec.Matches(mobile))
        {
            lock (statusRequests)
            {
                var now = DateTime.UtcNow;
                if (statusRequests.TryGetValue(mobile.Id, out DateTime lastRequestTime))
                {
                    if (now - lastRequestTime < TimeSpan.FromSeconds(1))
                        return;
                }
                
                UO.Log($"Requesting status of {mobile.Id}");
                UO.RequestStatus(mobile);
                
                statusRequests[mobile.Id] = now;
            }
        }
    }
    
    private static void HandleMobileLeftView(object sender, Mobile mobile)
    {
        if (PetsSpec.Matches(mobile) && statuses.Contains(mobile))
        {
            UO.Log($"Pet left view: {mobile}");
            statuses.Remove(mobile);
        }
    }
    
    public static IMobileLookup MyPets { get; } = new MobileLookupLinqWrapper(
        UO.Mobiles.Matching(PetsSpec).Where(x => x.CanModifyName));
        
    public static void Show() => statuses.Open();
}

UO.RegisterCommand("pets-show", Pets.Show);