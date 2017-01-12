#r "..\UltimaRX\bin\Debug\UltimaRX.dll"
#r "..\UltimaRX.Proxy\bin\Debug\UltimaRX.Proxy.exe"
#load "constants.csx"
#load "ItemTypes.cs"
#load "HarvestMapBuilder.cs"
#load "Scripts.cs"

using System;
using System.Threading;
using UltimaRX.Proxy;
using UltimaRX.Packets;
using UltimaRX.Proxy.InjectionApi;
using static UltimaRX.Proxy.InjectionApi.Injection;
using static Scripts;

Program.Start(currentConnection, 33333);
