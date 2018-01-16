using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

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
                UO.CommandHandler.Invoke("startup");
            }
        }        
    }
    
    public static void Wait()
    {
        if (!UO.IsLoginConfirmed)
        {
            Trace.Log("Login not confirmed, waiting for confirmation");
            var journal = UO.CreateEventJournal();
            
            journal.When<LoginConfirmedEvent>(ev => {})
                .WaitAny(TimeSpan.MaxValue);                
        }
        
        Trace.Log("Login confirmed, waiting for client stabilization");
        Thread.Sleep(5000);

        Launch();
    }
    
    private static void Launch()
    {
        isReady = true;
    
        foreach (var command in startupCommands)
        {
            Trace.Log($"Launching command {command}");
            UO.CommandHandler.Invoke(command);
        }
    }
}

UO.RegisterBackgroundCommand("startup", Startup.Wait);