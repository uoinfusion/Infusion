#r "Infusion.Scripts.UOErebor.Extensions.dll"

#load "Specs.csx"
#load "common.csx"
#load "RequestStatusQueue.csx"

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Infusion.Scripts.UOErebor.Extensions.StatusBars;

public static class Pets
{
    private static readonly Statuses statuses;
    private static bool enabled;
    private static readonly RequestStatusQueue requestStatusQueue = new RequestStatusQueue();

    public static MobileSpec PetsSpec = new[] { Specs.NecroSummons }; 

    public static IMobileLookup MyPets { get; } = new MobileLookupLinqWrapper(
        UO.Mobiles.Matching(PetsSpec).Where(x => x.CanRename));

    static Pets()
    {
        statuses = new Statuses("Pets");
        statuses.MobileTargeted += (sender, id) =>
        {
            var target = UO.Mobiles[id];
            if (target != null)
                UO.Target(target);
        };
        
        UO.Events.MobileEnteredView += HandleMobileEnteredView;
        UO.Events.MobileLeftView += HandleMobileLeftView;
        UO.Events.MobileDeleted += HandleMobileLeftView;
        UO.Events.HealthUpdated += HandleHealthUpdated;

        requestStatusQueue.StartProcessing();
    }
    
    public static void Enable()
    {
        if (!enabled)
        {
            statuses.Clear();
            enabled = true;

            AddMyPets();
            if (statuses.Count > 0)
                statuses.Open();
        }
    }
    
    public static void Disable()
    {
        if (enabled)
        {
            statuses.Close();
            enabled = false;
        }
    }

    private static void AddMyPets()
    {
        foreach (var potentialPet in UO.Mobiles.Matching(PetsSpec))
        {
            if (!potentialPet.CanRename)
                requestStatusQueue.RequestStatus(potentialPet.Id);
        }
    
        foreach (var pet in MyPets)
        {
            statuses.Add(pet, StatusBarType.Pet);
        }
    }

    private static void HandleHealthUpdated(object sender, CurrentHealthUpdatedEvent args)
    {
        if (enabled)
        {
            if (statuses.Contains(args.UpdatedMobile))
            {
                statuses.Update(args.UpdatedMobile);
            }
            else if (args.UpdatedMobile.CanRename)
            {
                if (statuses.Count == 0)
                    AddMyPets();
    
                statuses.Add(args.UpdatedMobile, StatusBarType.Pet);
            }
        }
    }
    
    private static void HandleMobileEnteredView(object sender, Mobile mobile)
    {
        if (PetsSpec.Matches(mobile))
        {
            requestStatusQueue.RequestStatus(mobile.Id);
        }
    }
    
    private static void HandleMobileLeftView(object sender, Mobile mobile)
    {
        if (enabled)
        {
            if (PetsSpec.Matches(mobile) && statuses.Contains(mobile))
            {
                UO.Log($"Pet left view: {mobile}");
                statuses.Remove(mobile);
            }
        }
    }    
        
    public static void Show() => statuses.Open();
}

UO.RegisterCommand("pets-show", Pets.Show);
UO.RegisterCommand("pets-enable", Pets.Enable);
UO.RegisterCommand("pets-disable", Pets.Disable);


