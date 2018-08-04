#load "common.csx"

using System.Collections.Generic;

public interface IContainer
{
    ObjectId Id { get; }
    Item Item { get; }
    void Open();
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
    private readonly IContainer parentContainer;

    public ObjectId Id { get; }
    public Item Item => UO.Items[Id];

    public Container(ObjectId containerId)
    {
        this.Id = containerId;
    }
    
    public Container(IContainer parentContainer, ObjectId containerId)
        : this(containerId)
    {
        this.parentContainer = parentContainer;
    }

    public void Open()
    {
        if (OpenContainerTracker.IsOpen(Id))
            return;
    
        if (parentContainer != null)
            parentContainer.Open();

        Common.OpenContainer(Id);
        OpenContainerTracker.SetOpen(Id);
    }
}


UO.RegisterBackgroundCommand("container-track", OpenContainerTracker.Track);
UO.CommandHandler.Invoke("container-track");
