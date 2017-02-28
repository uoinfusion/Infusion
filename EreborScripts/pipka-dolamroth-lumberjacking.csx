#load "Scripts.csx"
#load "PipkaDolAmroth.csx"
#load "MapRecorder.csx"

using Infusion.Proxy.LegacyApi;
using static Scripts;

Legacy.CommandHandler.RegisterCommand(new Command("lumber", PipkaDolAmroth.DolAmroth));