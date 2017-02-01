#r "..\UltimaRX\bin\Debug\UltimaRX.dll"
#r "..\UltimaRX.Proxy\bin\Debug\UltimaRX.Proxy.exe"
#r "..\UltimaRX.Nazghul.Proxy\bin\Debug\UltimaRX.Nazghul.Proxy.dll"
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
using UltimaRX.Nazghul.Proxy;
using static UltimaRX.Proxy.InjectionApi.Injection;
using static Scripts;

void Start()
{
    Program.Start(currentConnection, 33334);
}

void StartNazghul()
{
    Program.Start(currentConnection, 33334);
    var nazghulProxy = new NazghulProxy("http://localhost:9094/");
}

