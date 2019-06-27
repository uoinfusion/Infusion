using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Infusion.Commands;

public static class Startup
{
    public static bool isReady;
    public static bool waitingStarted;
    public static object waitingLock = new object();
    private static List<string> startupCommandNames = new List<string>();
    private static List<Action> startupActions = new List<Action>();
    
    public static ScriptTrace Trace { get; } = UO.Trace.Create();

    public static void RegisterStartupCommand(string commandName)
    {
        if (isReady)
        {
            Trace.Log($"Invoking command '{commandName}' directly");
            Invoke(commandName);
        }
        else
        {
            Trace.Log($"Adding command '{commandName}' to queue");
            startupCommandNames.Add(commandName);
            
            StartWaiting();
        }
    }
    
    public static void RunInGame(Action action)
    {
        if (isReady)
        {
            action.Invoke();
        }
        else
        {
            startupActions.Add(action);
            
            StartWaiting();
        }
    }
    
    private static void StartWaiting()
    {
        lock (waitingLock)
        {
            if (!waitingStarted)
            {
                Trace.Log("Starting startup command");
                UO.CommandHandler.Invoke("startup-handling", CommandExecutionMode.AlwaysParallel);
                waitingStarted = true;
            }
        }        
    }
    
    public static void Wait()
    {
        var journal = UO.CreateEventJournal();

        if (!UO.IsLoginConfirmed)
        {
            bool confirmed = false;
            journal.When<LoginConfirmedEvent>(ev => { confirmed = true;})
                .All();                

            if (!confirmed)
            {
                Trace.Log("Login not confirmed, waiting for confirmation");
                journal.When<LoginConfirmedEvent>(ev => {})
                    .WaitAny(TimeSpan.MaxValue);
            }                

            Trace.Log("Login confirmed, waiting for client stabilization");
            Thread.Sleep(5000);
        }

        Launch();
    }
    
    private static void Launch()
    {
        isReady = true;
    
        foreach (var action in startupActions)
        {
            action.Invoke();
        }
    
        foreach (var commandName in startupCommandNames)
        {
            Invoke(commandName);
        }
    }
    
    private static void Invoke(string commandName)
    {
        if (UO.CommandHandler.TryGetCommand(commandName, out Command command))
        {
            Trace.Log($"Launching startup command {commandName}");
            
            var mode = (command.ExecutionMode != CommandExecutionMode.Background) 
                ? CommandExecutionMode.AlwaysParallel
                : CommandExecutionMode.Background;

            UO.CommandHandler.Invoke(commandName, CommandExecutionMode.AlwaysParallel);
        }
        else
        {
            UO.Log($"Cannot invoke unknown command '{commandName}'."); 
        }
    }
}

UO.RegisterCommand("startup-handling", Startup.Wait);