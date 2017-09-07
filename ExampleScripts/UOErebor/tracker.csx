#load "Specs.csx"
#load "common.csx"

using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Infusion;
using System.Collections;

public static class Tracker
{
    public static MobileSpec PetsSpec = new[] { Specs.NecroSummons }; 

    public static void Initialize()
    {
        UO.Events.MobileEnteredView += HandleMobileEnteredView;
    }
   
    private static void HandleMobileEnteredView(object sender, MobileEnteredViewArgs args)
    {
        if (PetsSpec.Matches(args.NewMobile))
            Task.Run(() => {
                UO.Log($"Requesting status of {args.NewMobile.Id}");
                UO.RequestStatus(args.NewMobile);
            });
    }
    
    public static IMobileLookup MyPets { get; } = new MobileLookupLinqWrapper(
        UO.Mobiles.Matching(PetsSpec).Where(x => x.CanModifyName));
}

Tracker.Initialize();