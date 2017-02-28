#load "ItemTypes.csx"
#load "Scripts.csx"
#load "common.csx"
#load "cooking.csx"
#load "looting.csx"
#load "MapRecorder.csx"
#load "commands.csx"

using System;
using System.Threading;
using Infusion.Proxy;
using Infusion.Packets;
using Infusion.Proxy.LegacyApi;
using Infusion.Packets.Parsers;
using Infusion.Gumps;
using static Infusion.Proxy.LegacyApi.Injection;
using static Scripts;

//void Start()
//{
//    Program.Start(new System.Net.IPEndPoint(System.Net.IPAddress.Parse("89.185.244.24"), 2593), 33334);
//}
