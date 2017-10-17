public class TwoStateAbility
{
    public bool? IsTurnedOn { get; private set; }
    private SpeechJournal abilityJournal = UO.CreateSpeechJournal();
    
    public string TurnedOffMessage { get; }
    public string TurnedOnMessage { get; }
    public string ToggleCommand { get; }
    
    public TwoStateAbility(string toggleCommand, string turnedOnMessage, string turnedOffMessage)
    {
        ToggleCommand = toggleCommand;
        TurnedOnMessage = turnedOnMessage;
        TurnedOffMessage = turnedOffMessage;
    }
    
    public void TurnOn()
    {
        if (IsTurnedOn.HasValue && IsTurnedOn.Value)
        {
            if (!abilityJournal.Contains(TurnedOffMessage))
                return;
        }
    
        UO.Say(ToggleCommand);
        abilityJournal
            .When(TurnedOnMessage, () => { })
            .When(TurnedOffMessage, () =>
            {
                UO.Say(ToggleCommand);
                abilityJournal
                    .When(TurnedOffMessage, () => UO.Alert($"Waning: cannot turn on {ToggleCommand}"))
                    .When(TurnedOnMessage, () => { })
                    .WaitAny();
            })
            .WaitAny();

        IsTurnedOn = true;
        abilityJournal.Delete();
    }
    
    public void TurnOff()
    {
        if (IsTurnedOn.HasValue && !IsTurnedOn.Value)
        {
            if (!abilityJournal.Contains(TurnedOnMessage))
                return;
        }

        UO.Say(ToggleCommand);
        abilityJournal
            .When(TurnedOnMessage, () =>
            {
                UO.Say(ToggleCommand);
                abilityJournal
                    .When(TurnedOnMessage, () => UO.Log($"Warning: cannot turn off {ToggleCommand}"))
                    .When(TurnedOffMessage, () => { })
                    .WaitAny();
            })
            .When(TurnedOffMessage, () => { })
            .WaitAny();

        IsTurnedOn = false;
        abilityJournal.Delete();
    }
}