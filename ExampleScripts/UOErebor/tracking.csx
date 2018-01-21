public static class Tracking
{
    public static void Track(string kind)
    {
        UO.UseSkill(Skill.Tracking);
        UO.WaitForDialogBox();
        UO.TriggerDialogBox(kind);
    }
}

UO.RegisterCommand("track-animals", () => Tracking.Track("Animals"));
UO.RegisterCommand("track-monsters", () => Tracking.Track("Monsters"));
UO.RegisterCommand("track-humans", () => Tracking.Track("Humans"));
UO.RegisterCommand("track-players", () => Tracking.Track("Players"));
UO.RegisterCommand("track-all", () => Tracking.Track("Anything that moves"));
