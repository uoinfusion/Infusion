public static class Craft
{
    public static void VigourOn()
    {
        UO.Say(".vigour");
        UO.Journal
            .When("Nyni jsi schopen nalezt lepsi materialy.", () => { })
            .When("Jsi zpatky v normalnim stavu.", () =>
            {
                UO.Say(".vigour");
                UO.Journal.WaitAny("Nyni jsi schopen nalezt lepsi materialy.",
                    "Jsi zpatky v normalnim stavu.");
            })
            .WaitAny();
    }

    public static void VigourOff()
    {
        UO.Say(".vigour");
        UO.Journal
            .When("Nyni jsi schopen nalezt lepsi materialy.", () =>
            {
                UO.Say(".vigour");
                UO.Journal.WaitAny("Nyni jsi schopen nalezt lepsi materialy.");
            })
            .When("Jsi zpatky v normalnim stavu.", () => { })
            .WaitAny();
    }
}