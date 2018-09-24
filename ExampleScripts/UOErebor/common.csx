#load "Specs.csx"

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using Infusion.LegacyApi;
using Infusion;
using Infusion.Commands;

public static class Common
{
    private static EventJournal commonJurnal = UO.CreateEventJournal();    

    public static void WaitForChangedLocation()
    {
        UO.Log("Waiting for changed location.");
        commonJurnal.When<PlayerLocationChangedEvent>(e => { })
            .WaitAny();

        UO.Log("Waiting for changed location finished.");
    }
    
    public static bool WaitForContainer()
    {
        bool result = false;
    
        commonJurnal.When<ContainerOpenedEvent>(e => result = true)
            .When<SpeechReceivedEvent>(e => e.Speech.Message.Contains("You cannot reach that"), e => result = false)
            .When<SpeechReceivedEvent>(e => e.Speech.Message.Contains("You can't see that"), e => result = false)
            .WaitAny();

        UO.Wait(50);
            
        return result;
    }
    
    public static void OpenContainerCommand(string parameters)
    {
        if (!uint.TryParse(parameters, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint containerId))
        {
            throw new CommandInvocationException("Expecting hexadecimal id of a container.");
        }
        
        OpenContainer(containerId);
    }
    
    public static void OpenContainer(ObjectId containerId)
    {
        var container = UO.Items[containerId];
        if (container == null)
        {
            throw new CommandInvocationException($"Cannot find container {containerId}.");
        }
        
        OpenContainer(container);
    }
    
    public static void OpenContainer(GameObject container)
    {
        UO.Use(container);
        if (!WaitForContainer())
        {
            throw new CommandInvocationException($"Cannot open container {container}");
        }
    }
    
    public static Item AskForContainer(string prompt)
    {
        UO.ClientPrint(prompt);
        var item = UO.AskForItem();
        
        if (item == null)
        {
            UO.ClientPrint("Targeting canceled");
            return null;
        }
        
        if (!Specs.Container.Matches(item))
        {
            UO.ClientPrint("Selected item is not a container");
            return null;
        }
        
        return item;
    }
    
    public static void WaitCommand(string parameters)
    {
        if (string.IsNullOrEmpty(parameters))
            throw new CommandInvocationException("Wait time not specified");
            
        if (!int.TryParse(parameters, out int waitMilliseconds))
            throw new CommandInvocationException($"{parameters}* is not a number");
            
        UO.Log($"Waiting {waitMilliseconds}"); 
        UO.Wait(waitMilliseconds);
        UO.Log("Waiting finished");
    }
    
    public static void InvisItem()
    {
        var targetInfo = UO.Info();
        if (targetInfo.HasValue)
        {
            if (targetInfo.Value.Id.HasValue)
            {
                UO.Client.DeleteItem(targetInfo.Value.Id.Value);
            }
            else
                UO.ClientPrint("No object selected");
        }
        else
            UO.ClientPrint("Targeting cancelled");
    }
    
    public static void InvisAllItems()
    {
        foreach (var item in UO.Items.OnGround())
            UO.Client.DeleteItem(item.Id);
        
        foreach (var mobile in UO.Mobiles)
        {
            UO.Client.DeleteItem(mobile.Id);
        }
    }
}

class MobileLookupLinqWrapper : IMobileLookup
{
    private IEnumerable<Mobile> enumerable;

    public MobileLookupLinqWrapper(IEnumerable<Mobile> enumerable)
    {
        this.enumerable = enumerable;
    }

    public Mobile this[ObjectId id] => enumerable.SingleOrDefault(x => x.Id == id);

    public bool Contains(ObjectId id) => enumerable.Any(x => x.Id == id);

    public IEnumerator<Mobile> GetEnumerator() => enumerable.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => enumerable.GetEnumerator();
}

public interface IMobileLookup : IEnumerable<Mobile>
{
    bool Contains(ObjectId id);
    Mobile this[ObjectId id] { get; }
}

UO.RegisterCommand("wait", Common.WaitCommand);
UO.RegisterCommand("opencontainer", Common.OpenContainerCommand);
UO.RegisterCommand("invis-item-all", Common.InvisAllItems);
UO.RegisterCommand("invis-item", Common.InvisItem);