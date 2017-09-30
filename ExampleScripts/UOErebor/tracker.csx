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
    public static IMobileLookup Enemies { get; } = new MobileLookupLinqWrapper(
        UO.Mobiles.Where(m => m.Notoriety == Notoriety.Enemy || m.Notoriety == Notoriety.Murderer)); 
}
