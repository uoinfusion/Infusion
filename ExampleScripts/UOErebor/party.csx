#r "Infusion.Scripts.UOErebor.Extensions.dll"
#load "RequestStatusQueue.csx"

using System;
using System.Collections.Generic;
using System.Linq;
using Infusion.LegacyApi.Events;
using Infusion.Scripts.UOErebor.Extensions.StatusBars;

public static class Party
{
    private static readonly Statuses statuses;
    private static readonly RequestStatusQueue requestStatusQueue =
        new RequestStatusQueue();

    public static StatusesConfiguration Window => statuses.Configuration;
    public static ScriptTrace Trace { get; } = UO.Trace.Create();

    private static Dictionary<ObjectId, string> memberIds = new Dictionary<ObjectId, string>();

    private static ObjectId? lastTargetId = null; 

    static Party()
    {
        requestStatusQueue.OneRequestInterval = TimeSpan.FromMilliseconds(500);
        statuses = Statuses.Create("Party", () => UO.ClientWindow);
        statuses.MobileTargeted += (sender, id) =>
        {
            lastTargetId = id;
            UO.Target(id);
        };

        requestStatusQueue.StartProcessing();
    }
    
    public static void Enable()
    {
        UO.CommandHandler.Invoke("party");
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
            .When<MobileLeftViewEvent>(HandleMobileLeftView)
            .When<MobileFlagsUpdatedEvent>(HandleMobileFlagsUpdated)
            .Incomming();
    }

    private static void HandleMobileFlagsUpdated(MobileFlagsUpdatedEvent ev)
    {
        if (statuses.Contains(ev.Updated))
        {
            Trace.Log($"Flags updated: {ev.Updated.IsDead}, {ev.Updated.Type}"); 
            Trace.Log($"Flags updated: {ev.BeforeUpdate.IsDead}, {ev.BeforeUpdate.Type}"); 
            statuses.Update(ev.Updated);
        }
    }

    private static void HandleMobileEnteredView(MobileEnteredViewEvent ev)
    {
        if (statuses.Contains(ev.Mobile))
        {
            requestStatusQueue.RequestStatus(ev.Mobile.Id);
            statuses.SetOutOfSight(ev.Mobile.Id, false);
            statuses.Update(ev.Mobile);
        }
        else
        {
            if (memberIds.TryGetValue(ev.Mobile.Id, out string prefix))
            {
                Add(ev.Mobile, prefix);
            }
        }
    }

    private static void HandleMobileLeftView(MobileLeftViewEvent ev)
    {
        if (statuses.Contains(ev.Mobile))
        {
            statuses.SetOutOfSight(ev.Mobile.Id, true);
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
    
    public static void ClearStatuses()
    {
        statuses.Clear();
    }
    
    public static void Add()
    {
        while (true)
        {
            UO.ClientPrint("Select a new party member, or press escape to cancel");
            var newMember = UO.AskForMobile();
            if (newMember == null)
                break;
    
            Add(newMember);
        }
    }
    

    public static void Add(Mobile newMember, string namePrefix = null)
    {
        if (!statuses.Contains(newMember))
        {
            UO.RequestStatus(newMember);

            UO.Log(namePrefix);
            statuses.Add(newMember, StatusBarType.Friend, namePrefix);
        }
        else
        {
            statuses.Open();
        }
    }
    
    public static void Add(params ObjectId[] ids)
    {
        foreach (var id in ids) 
            memberIds[id] = null;
    }
    
    public static void Add(ObjectId id, string prefix)
    {
        memberIds[id] = prefix;
    }
    
    public static void ClearMembers()
    {
        memberIds.Clear();
    }
    
    public static void Remove()
    {
        var member = UO.AskForMobile();
        if (member == null)
        {
            if (lastTargetId.HasValue)
            {
                memberIds.Remove(lastTargetId.Value);
                statuses.Remove(lastTargetId.Value);
                lastTargetId = null;
            }
            return;
        }
        else
        {
            statuses.Remove(member);
            memberIds.Remove(member.Id);
        }
    }
    
    public static void Remove(Mobile removeMember)
    {
       if (statuses.Contains(removeMember))
       {       

           statuses.Remove(removeMember);
       }
    }
    
    public static void WindowInfo()
    {
        UO.Log(statuses.WindowInfo);
    }

}

UO.RegisterBackgroundCommand("party", Party.Run);
UO.RegisterCommand("party-enable",Party.Enable);
UO.RegisterCommand("party-disable", Party.Disable);
UO.RegisterCommand("party-toggle", Party.Toggle);
UO.RegisterCommand("party-add", Party.Add);
UO.RegisterCommand("party-remove", Party.Remove);
UO.RegisterCommand("party-show", Party.ShowStatuses);
UO.RegisterCommand("party-windowinfo", Party.WindowInfo);
