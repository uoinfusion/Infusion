#load "startup.csx"

using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using Events=Infusion.LegacyApi.Events;

public class TwoStateAbility : ITwoStateAbility
{
    private static ImmutableList<TwoStateAbility> abilities = ImmutableList<TwoStateAbility>.Empty;
    private static EventJournal journal = UO.CreateEventJournal();

    public bool? IsTurnedOn { get; private set; }
    private SpeechJournal abilityJournal = UO.CreateSpeechJournal();

    public string TurnedOffMessage { get; }
    public string TurnedOnMessage { get; }
    public string[] FailMessages { get; }
    public string ToggleCommand { get; }
    
    public StateIndicator Indicator { get; }
    
    public int LowStaminaThreshold { get; set; } = 5;
    
    public TwoStateAbility(string toggleCommand, string turnedOnMessage, string turnedOffMessage)
        : this(toggleCommand, turnedOnMessage, turnedOffMessage, Array.Empty<string>())
    {
    }
    
    public TwoStateAbility(string toggleCommand, string turnedOnMessage, string turnedOffMessage,
        string[] failMessages, StateIndicator indicator = null)
    {
        ToggleCommand = toggleCommand;
        TurnedOnMessage = turnedOnMessage;
        TurnedOffMessage = turnedOffMessage;
        FailMessages = failMessages;
        Indicator = indicator;

        abilities = abilities.Add(this);
    }

    public void TrackState(string message)
    {
        if (TurnedOnMessage.IndexOf(message, StringComparison.OrdinalIgnoreCase) >= 0)
        {
            IsTurnedOn = true;
            Indicator?.Show();
        }
        else if (TurnedOffMessage.IndexOf(message, StringComparison.OrdinalIgnoreCase) >= 0)
        {
            IsTurnedOn = false;
            Indicator?.Hide();
        }
    }

    public static void TrackingCycle()
    {
        journal
            .When<Events.SpeechReceivedEvent>(entry => {
                foreach (var ability in abilities)
                {
                    ability.TrackState(entry.Speech.Message);
                }
            })
            .Incomming();
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

public class StateIndicator
{
    private string gumpCommands;
    
    public GumpInstanceId GumpId { get; }
    public GumpTypeId TypeId { get; }
    public int PictureId { get; }
    public int X { get; }
    public int Y { get; }
    
    public StateIndicator(uint gumpId, uint typeId, int pictureId, int x, int y)
    {
        GumpId = (GumpInstanceId)(uint)gumpId;
        TypeId = (GumpTypeId)(uint)typeId;
        PictureId = pictureId;
        X = x;
        Y = y;
        
        var builder = new StringBuilder();
        builder.Append(@"{NoClose 1}{GumpPic 0 0 ");
        builder.Append(PictureId);
        builder.Append('}');
        
        gumpCommands = builder.ToString();
    }
    
    public void Show()
    {
        UO.Client.ShowGump(GumpId, TypeId, X, Y, gumpCommands, null); 
    }
    
    public void Hide()
    {
        UO.Client.CloseGump(TypeId);
    }
}

UO.RegisterBackgroundCommand("track-twostateabilities", TwoStateAbility.TrackingCycle);
Startup.RegisterStartupCommand("track-twostateabilities");