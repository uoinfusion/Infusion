#r "Infusion.Scripts.UOErebor.Extensions.dll"

using Infusion.Scripts.UOErebor.Extensions.StatusBars;

public static class Party
{
    private static readonly Statuses statuses;

    static Party()
    {
        statuses = new Statuses();
        UO.Events.HealthUpdated += HandleHealthUpdated; 
    }
    
    private static void HandleHealthUpdated(object sender, CurrentHealthUpdatedArgs args)
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
