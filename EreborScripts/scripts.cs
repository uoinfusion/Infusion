using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UltimaRX.Packets;
using UltimaRX.Proxy.InjectionApi;
using static UltimaRX.Proxy.InjectionApi.Injection;

public static class Scripts
{
    public static Action MassKill = Script.Create(ScriptImplementation.MassKill);
    public static Action Cook = Script.Create(ScriptImplementation.Cook);
    public static Action Loot = Script.Create(ScriptImplementation.Loot);
    public static Action DolAmrothLumber1 = Script.Create(() => ScriptImplementation.Harvest("dolamroth-lumberjacking.map"));
    public static Action DolAmrothLumber2 = Script.Create(() => ScriptImplementation.Harvest("dolamroth-lumberjacking2.map"));
    public static Action DolAmrothKilling = Script.Create(() => ScriptImplementation.Harvest("dolamroth-killing.map"));
}
