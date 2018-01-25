public static class ExplevelTracker
{
    public static int Level { get; private set; }
    public static int CurrentExp { get; private set; }
    public static int NextLevelExp { get; private set; }

    public static void Refresh()
    {
        UO.Log("Refreshing explevel info...");
        UO.Say(".explevel");
        var gump = UO.WaitForGump(false);
        Parse(gump);
        UO.GumpResponse().Cancel();
    }
    
    private const string expGainMessageSuffix = " zkusenosti"; 
    private const string expGainMessagePrefix = "Ziskal jsi ";
    private static readonly GumpTypeId explevelGumpId = (GumpTypeId)0x96000553;    

    public static void Enable()
    {
        UO.CommandHandler.Invoke("explevel-run");
    }
    
    public static void Disable()
    {
        UO.CommandHandler.Terminate("explevel-run");
    }

    public static void Run()
    {
        var journal = UO.CreateEventJournal();
        
        journal
            .When<SpeechReceivedEvent>(
                e => e.Speech.Message.StartsWith(expGainMessagePrefix),
                e => ParseExpGainMessage(e.Speech.Message))
            .When<GumpReceivedEvent>(
                e =>
                {
                    UO.Log($"{e.Gump.Id}, {e.Gump.GumpTypeId}");
                    return e.Gump.GumpTypeId.Equals(explevelGumpId);
                },
                e => Parse(e.Gump))
            .Incomming();
    }

    // Example: Ziskal jsi 44 zkusenosti.
    public static void ParseExpGainMessage(string message)
    {
        if (message.StartsWith(expGainMessagePrefix))
        {
            if (Level == 0 && NextLevelExp == 0 && CurrentExp == 0)
            {
                Refresh();
                return;
            }
        
            var expGainMessageParts = message.Split(' ');
            if (expGainMessageParts.Length != 4)
            {
                UO.Log($"Not a exp gain message: {message}");
                return;
            }
            
            int expGain = int.Parse(expGainMessageParts[2]);
            
            if (expGain + CurrentExp > NextLevelExp)
            {
                Refresh();
            }
            else
            {
                CurrentExp += expGain;
            }
        }
        else
        {
            UO.Log($"Message is not an exp gain message.");
            UO.Log(message);
        }
    }

    public static void Parse(Gump gump)
    {
        if (gump == null && !gump.GumpTypeId.Equals(explevelGumpId))
        {
            UO.Log("The gump is not .explevel gump");
            return;
        }

        if (gump.TextLines.Length < 2)
        {
            UO.Log($"The gump is incorrect. Expecting 2 or more text lines but the gump has {gump.TextLines.Length}");
            return;
        }

        string experienceValueText = gump.TextLines[2];
        if (string.IsNullOrEmpty(experienceValueText) || !experienceValueText.Contains("/"))
        {
            UO.Log($"The gump is incorrect. Expecting exp value on line 2, but {experienceValueText} found.");
            return;
        }
        string[] experienceValueParts = experienceValueText.Split('/');
        if (experienceValueParts.Length != 2)
        {
            UO.Log($"Cannot parse experience text is ${experienceValueText}");
            return;
        }

        if (!int.TryParse(experienceValueParts[0].Trim(), out int currentExp))
        {
            UO.Log($"Cannot parse experience text is ${experienceValueText}");
            return;
        }
        CurrentExp = currentExp;
        
        if (!int.TryParse(experienceValueParts[1].Trim(), out int nextLevelExp))
        {
            UO.Log($"Cannot parse experience text is ${experienceValueText}");
            return;
        }
        NextLevelExp = nextLevelExp;

        string levelValueText = gump.TextLines[4];
        if (!int.TryParse(levelValueText, out int level))
        {
            UO.Log($"Cannot parse level text is ${levelValueText}");
            return;
        }
        Level = level;
    }
}

UO.RegisterBackgroundCommand("explevel-run", ExplevelTracker.Run);
UO.RegisterCommand("explevel-enable", ExplevelTracker.Enable);
UO.RegisterCommand("explevel-disable", ExplevelTracker.Disable);
UO.RegisterCommand(",explevel", () => UO.Say(".explevel"));
UO.RegisterCommand(",explevel-refresh", ExplevelTracker.Refresh);
