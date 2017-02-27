#load "ItemTypes.csx"
#load "Scripts.csx"
#load "common.csx"
#load "cooking.csx"
#load "looting.csx"
#load "MapRecorder.csx"
#load "commands.csx"

using System;
using System.Threading;
using UltimaRX.Proxy;
using UltimaRX.Packets;
using UltimaRX.Proxy.InjectionApi;
using UltimaRX.Packets.Parsers;
using UltimaRX.Gumps;
using static UltimaRX.Proxy.InjectionApi.Injection;
using static Scripts;

//void Start()
//{
//    Program.Start(new System.Net.IPEndPoint(System.Net.IPAddress.Parse("89.185.244.24"), 2593), 33334);
//}
