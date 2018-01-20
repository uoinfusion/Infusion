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
    public static List<string> startupCommands = new List<string>();
    
    public static ScriptTrace Trace { get; } = UO.Trace.Create();

    public static void RegisterStartupCommand(string commandName)
    {
        if (isReady)
        {
            Trace.Log($"Invoking command '{commandName}' directly");
            UO.CommandHandler.Invoke(commandName);
        }
        else
        {
            Trace.Log($"Adding command '{commandName}' to queue");
            startupCommands.Add(commandName);
            
            StartWaiting();
        }
    }
    
    private static void StartWaiting()
    {
        lock (waitingLock)
        {
            if (!UO.CommandHandler.IsCommandRunning("startup"))
            {
                Trace.Log("Starting startup command");
                UO.CommandHandler.Invoke("startup-handling", CommandExecutionMode.AlwaysParallel);
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
    
        foreach (var command in startupCommands)
        {
            Trace.Log($"Launching startup command {command}");
            UO.CommandHandler.Invoke(command, CommandExecutionMode.AlwaysParallel);
        }
    }
}

UO.RegisterCommand("startup-handling", Startup.Wait);