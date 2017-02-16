#load "configuration.csx"
#load "ItemTypes.cs"
#load "Scripts.cs"
#load "PipkaDolAmroth.cs"
#load "MapRecorder.cs"

using System;
using System.Threading;
using UltimaRX.Proxy;
using UltimaRX.Packets;
using UltimaRX.Proxy.InjectionApi;
using UltimaRX.Packets.Parsers;
using UltimaRX.Gumps;
using static UltimaRX.Proxy.InjectionApi.Injection;
using static Scripts;

Injection.CommandHandler.RegisterCommand(new Command("masskill", MassKill));
Injection.CommandHandler.RegisterCommand(new Command("loot", Loot));
