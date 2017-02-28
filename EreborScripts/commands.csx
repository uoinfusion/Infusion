#load "Scripts.csx"

using Infusion.Proxy.LegacyApi;
using static Scripts;

Legacy.CommandHandler.RegisterCommand(new Command("masskill", MassKill));
Legacy.CommandHandler.RegisterCommand(new Command("fish", () => Fish()));