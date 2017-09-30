using System;
using System.Linq;

public static class Afk
{
    private class AfkToken : IDisposable
    {
        public void Dispose()
        {
            Afk.Stop();
        }
    }

    public static string[] AfkNames { get; set; } =
    {
        "desttro", "elbereth", "finn", "gothmog", "houba", "iustus", "myke", "yavanna",
        "nightmare", "sirglorg", "ustus", "levtar", "lustus"
    };

    public static string[] AfkMessages { get; set; } =
    {
        "afk", "kontrola", "makro", "maker", "jsi t",
        "gm", "halo", "lagr"
    };
    
    private static GameJournal afkCheckJournal = UO.CreateJournal();
 
    public static IDisposable Start()
    {
        afkCheckJournal.Delete();
        
        return new AfkToken();
    }
    
    public static void Stop()
    {
        afkCheckJournal.Delete();
    }
 
    public static void Check()
    {
        if (IsAfkAlertRequired)
        {
            Alert();
        }
    }

    public static bool IsAfkAlertRequired
    {
        get
        {
            var afkAlertRequired = afkCheckJournal
                .ByAnyName(AfkNames).Any();

            afkAlertRequired |= afkCheckJournal.Contains(AfkMessages);

            afkCheckJournal.Delete();
        
            return afkAlertRequired;
        }
    }

    public static void Alert()
    {
        afkCheckJournal.Delete();
        while (true)
        {
            DoAlert();

            if (afkCheckJournal.Contains("tak zpet do prace"))
                break;
            afkCheckJournal.Delete();
        }
    }

    public static void DoAlert()
    {
        System.Media.SystemSounds.Asterisk.Play();
        UO.Alert("Afk kontrola");

        UO.Wait(1000);
    }

    public static void TestAlert()
    {
        for (int i = 0; i < 2; i++)
            DoAlert();
    }
}

UO.RegisterCommand("testafk", Afk.TestAlert);
