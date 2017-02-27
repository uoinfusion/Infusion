#load "Scripts.csx"

using Infusion.Proxy.InjectionApi;
using static Scripts;

Injection.CommandHandler.RegisterCommand(new Command("masskill", MassKill));
Injection.CommandHandler.RegisterCommand(new Command("fish", () => Fish()));