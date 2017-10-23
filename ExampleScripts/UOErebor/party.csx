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

        requestStatusQueue.StartProcessing();
    }
    
    public static void Enable()
    {
        UO.CommandHandler.Invoke(",party");
    }
    
    public static void Disable()
    {
        UO.CommandHandler.Terminate("party");
    }
    
    public static void Toggle()
    {
        if (UO.CommandHandler.IsCommandRunning("party"))
            Disable();
        else
            Enable();
    }
    
    public static void Run()
    {
        var journal = UO.CreateEventJournal();
        journal
            .When<CurrentHealthUpdatedEvent>(HandleHealthUpdated)
            .When<MobileEnteredViewEvent>(HandleMobileEnteredView)
            .Incomming();
    }
        
    private static void HandleMobileEnteredView(MobileEnteredViewEvent ev)
    {
        if (statuses.Contains(ev.Mobile))
        {
            requestStatusQueue.RequestStatus(ev.Mobile.Id);
        }
    }
    
    private static void HandleHealthUpdated(CurrentHealthUpdatedEvent ev)
    {
        if (statuses.Contains(ev.UpdatedMobile))
        {
            statuses.Update(ev.UpdatedMobile);
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

UO.RegisterBackgroundCommand("party", Party.Run);
UO.RegisterCommand("party-enable",Party.Enable);
UO.RegisterCommand("party-disable", Party.Disable);
UO.RegisterCommand("party-toggle", Party.Toggle);
UO.RegisterCommand("party-add", Party.Add);
UO.RegisterCommand("party-remove", Party.Remove);
UO.RegisterCommand("party-show", Party.ShowStatuses);
