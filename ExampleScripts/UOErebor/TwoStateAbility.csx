using System;

public class TwoStateAbility : ITwoStateAbility
{
    public bool? IsTurnedOn { get; private set; }
    private SpeechJournal abilityJournal = UO.CreateSpeechJournal();
    
    public string TurnedOffMessage { get; }
    public string TurnedOnMessage { get; }
    public string[] FailMessages { get; }
    public string ToggleCommand { get; }
    
    public int LowStaminaThreshold { get; set; } = 15;
    
    public TwoStateAbility(string toggleCommand, string turnedOnMessage, string turnedOffMessage)
        : this(toggleCommand, turnedOnMessage, turnedOffMessage, Array.Empty<string>())
    {
    }
    
    public TwoStateAbility(string toggleCommand, string turnedOnMessage, string turnedOffMessage, string[] failMessages)
    {
        ToggleCommand = toggleCommand;
        TurnedOnMessage = turnedOnMessage;
        TurnedOffMessage = turnedOffMessage;
        FailMessages = failMessages;
    }
    
    public void TurnOn()
    {
        if (UO.Me.CurrentStamina <= LowStaminaThreshold)
        {
            UO.Log("Low stamina, cannot turn on the ability.");
            return;
        }
    
        try
        {
            if (IsTurnedOn.HasValue && IsTurnedOn.Value)
            {
                // doesn't work if this method is called rarelly,
                // so a message is scrolled out from the journal
                if (!abilityJournal.Contains(TurnedOffMessage))
                {
                    abilityJournal.Delete();
                    return;
                }
            }
        
            UO.Say(ToggleCommand);
            abilityJournal
                .When(TurnedOnMessage, () => { })
                .When(TurnedOffMessage, () =>
                {
                    UO.Say(ToggleCommand);
                    abilityJournal
                        .When(TurnedOffMessage, () => UO.ClientPrint($"Waning: cannot turn on {ToggleCommand}", UO.Me))
                        .When(TurnedOnMessage, () => { })
                        .When(FailMessages, () => UO.ClientPrint($"Warning: {ToggleCommand} failed.", UO.Me))
                        .WaitAny();
                })
                .When(FailMessages, () => UO.ClientPrint($"Warning: {ToggleCommand} failed.", UO.Me))
                .WaitAny();
    
            IsTurnedOn = true;
            abilityJournal.Delete();
        }
        catch (System.Exception ex)
        {
            UO.Log(ex.ToString());
            throw;
        }
    }
    
    public void TurnOff()
    {
        try
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
                        .When(FailMessages, () => UO.ClientPrint($"Warning: {ToggleCommand} failed.", UO.Me))
                        .WaitAny();
                })
                .When(TurnedOffMessage, () => { })
                .When(FailMessages, () => UO.ClientPrint($"Warning: {ToggleCommand} failed.", UO.Me))
                .WaitAny();
    
            IsTurnedOn = false;
            abilityJournal.Delete();
        }
        catch (System.Exception ex)
        {
            UO.Log(ex.ToString());
            throw;
        }
    }
}

public interface ITwoStateAbility
{
    void TurnOn();
    void TurnOff();
}
    
class NoAbility : ITwoStateAbility
{
    public void TurnOff()
    {
    }

    public void TurnOn()
    {
    }
}