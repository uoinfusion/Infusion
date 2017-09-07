using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Infusion.LegacyApi;
using Infusion;
using Infusion.Commands;

public static class Common
{
    public static void WaitForChangedLocation()
    {
        Location3D startLocation = UO.Me.Location;
        do
        {
            UO.Wait(50);
        }
        while (UO.Me.Location == startLocation);

        UO.Log("Waiting for changed location finished.");
    }
    
    public static void WaitCommand(string parameters)
    {
        if (string.IsNullOrEmpty(parameters))
            throw new CommandInvocationException("Wait time not specified");
            
        if (!int.TryParse(parameters, out int waitMilliseconds))
            throw new CommandInvocationException($"{parameters} is not a number");
            
        UO.Log($"Waiting {waitMilliseconds}"); 
        UO.Wait(waitMilliseconds);
        UO.Log("Waiting finished");
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
