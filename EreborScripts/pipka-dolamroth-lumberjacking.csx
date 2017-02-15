#load "ItemTypes.cs"
#load "Scripts.cs"
#load "PipkaDolAmroth.cs"
#load "MapRecorder.cs"

using static Scripts;

Injection.CommandHandler.RegisterCommand(new Command("lumber", PipkaDolAmroth.DolAmroth));