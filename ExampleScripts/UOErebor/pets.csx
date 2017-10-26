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
        
        requestStatusQueue.StartProcessing();
    }
    
    public static void Run()
    {
        var journal = UO.CreateEventJournal();
        journal
            .When<MobileEnteredViewEvent>(e => HandleMobileEnteredView(e.Mobile))
            .When<MobileLeftViewEvent>(e => HandleMobileLeftView(e.Mobile))
            .When<MobileDeletedEvent>(e => HandleMobileLeftView(e.Mobile))
            .When<CurrentHealthUpdatedEvent>(HandleHealthUpdated)
            .Incomming();
    }
    
    public static void Enable()
    {
        if (!UO.CommandHandler.IsCommandRunning("pets"))
        {
            UO.CommandHandler.Invoke(",pets");
            statuses.Clear();

            AddMyPets();
            if (statuses.Count > 0)
                statuses.Open();
        }
    }
    
    public static void Disable()
    {
        if (UO.CommandHandler.IsCommandRunning("pets"))
        {
            UO.CommandHandler.Terminate("pets");
            statuses.Close();
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

    private static void HandleHealthUpdated(CurrentHealthUpdatedEvent args)
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
    
    private static void HandleMobileEnteredView(Mobile mobile)
    {
        if (PetsSpec.Matches(mobile))
        {
            requestStatusQueue.RequestStatus(mobile.Id);
        }
    }
    
    private static void HandleMobileLeftView(Mobile mobile)
    {
        if (PetsSpec.Matches(mobile) && statuses.Contains(mobile))
        {
            UO.Log($"Pet left view: {mobile}");
            statuses.Remove(mobile);
        }
    }    
        
    public static void Show() => statuses.Open();
}

UO.RegisterBackgroundCommand("pets", Pets.Run);
UO.RegisterCommand("pets-show", Pets.Show);
UO.RegisterCommand("pets-enable", Pets.Enable);
UO.RegisterCommand("pets-disable", Pets.Disable);


