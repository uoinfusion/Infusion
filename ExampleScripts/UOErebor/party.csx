#r "Infusion.Scripts.UOErebor.Extensions.dll"
#load "RequestStatusQueue.csx"

using System;
using Infusion.Scripts.UOErebor.Extensions.StatusBars;

public static class Party
{
    private static readonly Statuses statuses;
    private static readonly RequestStatusQueue requestStatusQueue =
        new RequestStatusQueue();

    static Party()
    {
        requestStatusQueue.OneRequestInterval = TimeSpan.FromMilliseconds(500);
        statuses = new Statuses("Party");
        statuses.MobileTargeted += (sender, id) =>
        {
            var target = UO.Mobiles[id];
            if (target != null)
                UO.Target(target);
        };

        UO.Events.HealthUpdated += HandleHealthUpdated;
        UO.Events.MobileEnteredView += HandleMobileEnteredView;

        requestStatusQueue.StartProcessing();
    }
        
    private static void HandleMobileEnteredView(object sender, Mobile mobile)
    {
        if (statuses.Contains(mobile))
        {
            requestStatusQueue.RequestStatus(mobile.Id);
        }
    }
    
    private static void HandleHealthUpdated(object sender, CurrentHealthUpdatedEvent args)
    {
        if (statuses.Contains(args.UpdatedMobile))
        {
            statuses.Update(args.UpdatedMobile);
        }
    }
    
    public static void ShowStatuses()
    {
        statuses.Open();
    }
    
    public static void Add()
    {
        var newMember = UO.AskForMobile();
        if (newMember == null)
            return;

        if (!statuses.Contains(newMember))
        {
            UO.RequestStatus(newMember);
            statuses.Add(newMember, StatusBarType.Friend);
        }
    }
    
    public static void Remove()
    {
        var member = UO.AskForMobile();
        if (member == null)
            return;
            
        statuses.Remove(member);
    }
}

UO.RegisterCommand("party-add", Party.Add);
UO.RegisterCommand("party-remove", Party.Remove);
UO.RegisterCommand("party-show", Party.ShowStatuses);
