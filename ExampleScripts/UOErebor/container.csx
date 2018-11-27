#load "common.csx"

using System.Linq;
using System.Collections.Generic;

public interface IContainer
{
    ObjectId Id { get; }
    Item Item { get; }

    void Open();
    bool Contains(Item item);
}

public static class OpenContainerTracker
{
    private static HashSet<ObjectId> openContainersCache = new HashSet<ObjectId>();
    private static EventJournal journal = UO.CreateEventJournal();

    public static void Track()
    {
        journal
            .When<PlayerLocationChangedEvent>(HandlePlayerLocationChange)
            .Incomming();
    }

    private static void HandlePlayerLocationChange(PlayerLocationChangedEvent obj)
    {
        lock (openContainersCache)
        {
            openContainersCache.Clear();
        }        
    }
    
    public static bool IsOpen(ObjectId id)
    {
        lock (openContainersCache)
        {
            return openContainersCache.Contains(id);
        }
    }
    
    public static void SetOpen(ObjectId id)
    {
        lock (openContainersCache)
        {
            openContainersCache.Add(id);
        }
    }
}

public class Container : IContainer
{
    public IContainer Parent { get; }
    public ObjectId Id { get; }
    public Item Item => UO.Items[Id];

    public Container(ObjectId containerId)
    {
        this.Id = containerId;
    }
    
    public Container(IContainer parentContainer, ObjectId containerId)
        : this(containerId)
    {
        this.Parent = parentContainer;
    }

    public void Open()
    {
        if (OpenContainerTracker.IsOpen(Id))
            return;
    
        if (Parent != null)
            Parent.Open();

        Common.OpenContainer(Id);
        OpenContainerTracker.SetOpen(Id);
    }
    
    public bool Contains(Item item)
        => item.ContainerId.HasValue && item.ContainerId.Value == Id;
        
    public bool InParentContainer
        => (Parent == null && !Item.ContainerId.HasValue)
            || (Parent != null && Parent.Contains(Item));
}

public class BackPackContainer : IContainer
{
    public ObjectId Id => UO.Me.BackPack.Id;

    public Item Item => UO.Me.BackPack;

    public bool Contains(Item item)
        => item.ContainerId.HasValue && item.ContainerId.Value == Id;

    public void Open()
    {
        if (UO.Me.BackPack == null)
        {
            UO.ClientPrint("Cannot open backpack.");
            return;
        }
    
        if (!OpenContainerTracker.IsOpen(UO.Me.BackPack.Id))
        {
            Common.OpenContainer(UO.Me.BackPack);
            OpenContainerTracker.SetOpen(UO.Me.BackPack.Id);
        }
    }
}

UO.RegisterBackgroundCommand("container-track", OpenContainerTracker.Track);
UO.CommandHandler.Invoke("container-track");
