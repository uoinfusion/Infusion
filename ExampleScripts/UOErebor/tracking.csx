public static class Tracking
{
    public static void Track(string kind)
    {
        UO.WarModeOff();
        UO.UseSkill(Skill.Tracking);
        if (UO.WaitForDialogBox("You are preoccupied with thoughts of battle.") != null)
        {
            UO.TriggerDialogBox(kind);
        }
    }
}

UO.RegisterCommand("track-animals", () => Tracking.Track("Animals"));
UO.RegisterCommand("track-monsters", () => Tracking.Track("Monsters"));
UO.RegisterCommand("track-humans", () => Tracking.Track("Humans"));
UO.RegisterCommand("track-players", () => Tracking.Track("Players"));
UO.RegisterCommand("track-all", () => Tracking.Track("Anything that moves"));
