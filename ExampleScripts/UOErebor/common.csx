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

UO.RegisterCommand("wait", Common.WaitCommand);
