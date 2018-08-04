#load "Specs.csx"
#load "items.csx"
#load "container.csx"

using System;
using System.Collections.Generic;
using System.Linq;

public class Warehouse
{
    private static Dictionary<ItemSpec, IContainer> locations = new Dictionary<ItemSpec, IContainer>();

    public static void AddLocation(ItemSpec spec, IContainer container)
    {
        locations.Add(spec, container);
    }
    
    public static IContainer GetContainer(ItemSpec requestedSpec)
    {
        foreach (var locationPair in locations)
        {
            if (locationPair.Key.Equals(requestedSpec))
            {
                return locationPair.Value;
            }
        }
        
        throw new InvalidOperationException($"No registered container for {Specs.TranslateToName(requestedSpec)} in your warehouse."); 
    }
}