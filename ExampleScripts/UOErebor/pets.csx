#r "Infusion.Scripts.UOErebor.Extensions.dll"

#load "Specs.csx"
#load "common.csx"
#load "RequestStatusQueue.csx"
#load "party.csx"

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Infusion.Scripts.UOErebor.Extensions.StatusBars;

public static class Pets
{
    private static readonly SpeechJournal journal = UO.CreateSpeechJournal();
    private static readonly Statuses statuses;
    private static readonly RequestStatusQueue requestStatusQueue = new RequestStatusQueue();

    public static MobileSpec PetsSpec = new[] { Specs.NecroSummons, Specs.MageSummons, Specs.Mounts, Specs.Dog, };
    public static StatusesConfiguration Window => statuses.Configuration;
    public static ScriptTrace Trace { get; } = UO.Trace.Create();

    public static IMobileLookup MyPets { get; } = new MobileLookupLinqWrapper(
        UO.Mobiles.Matching(PetsSpec).Where(x => x.CanRename));

    static Pets()
    {
        statuses = Statuses.Create("Pets", () => UO.ClientWindow);
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
            UO.CommandHandler.Invoke("pets");
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
        else if (args.UpdatedMobile.CanRename && PetsSpec.Matches(args.UpdatedMobile)) 
        {
            if (statuses.Count == 0)
            {
                AddMyPets();
            }

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
        if (statuses.Contains(mobile))
        {
            Trace.Log($"Pet left view: {mobile}");
            statuses.Remove(mobile);
        }
    }

    public static void WindowInfo()
    {
        UO.Log(statuses.WindowInfo);
    }

    public static void Show() => statuses.Open();

    public static void AllFriend()
    {
        if (!MyPets.Any())
        {
            UO.ClientPrint("No pets visible, cannot friend them to party");
            return;
        }
        
        if (!Party.VisibleMemberIds.Any())
        {
            UO.ClientPrint("No party members visible.");
            return;
        }

        foreach (var friendId in Party.VisibleMemberIds)
        {
            if (UO.Me.PlayerId == friendId)
            {
                UO.ClientPrint("Cannot give friend to myself");
                continue;
            }

            var friend = UO.Mobiles[friendId];
            if (friend != null)
            {
                UO.ClientPrint($"Giving friend to {friend.Name ?? friend.Id.ToString()}");
                UO.Say("all friend");
                UO.WaitForTarget();
                UO.Target(friend);
                UO.Wait(25);
            }
        }
        
        UO.ClientPrint("All friend finished");
    }
}

UO.RegisterBackgroundCommand("pets", Pets.Run);
UO.RegisterCommand("pets-show", Pets.Show);
UO.RegisterCommand("pets-windowinfo", Pets.WindowInfo);
UO.RegisterCommand("pets-enable", Pets.Enable);
UO.RegisterCommand("pets-disable", Pets.Disable);
UO.RegisterCommand("pets-allfriend-party", Pets.AllFriend);