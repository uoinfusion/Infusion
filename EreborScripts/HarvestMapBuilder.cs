using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UltimaRX.Proxy;
using static UltimaRX.Proxy.InjectionApi.Injection;

public class HarvestMapBuilder : IDisposable
{
    private readonly Stream file;

    private readonly Queue<string> fileOutputQueue = new Queue<string>();
    private readonly object harvestLock = new object();
    private readonly StreamWriter writer;
    private bool disposed;

    public HarvestMapBuilder(string fileName)
    {
        file = File.Create(fileName);
        writer = new StreamWriter(file)
        {
            AutoFlush = true
        };
        CommandReceived += HandleTraceBuildingCommand;
    }

    public void Dispose()
    {
        lock (harvestLock)
        {
            CommandReceived -= HandleTraceBuildingCommand;
            writer.Dispose();
            file.Dispose();
            disposed = true;
        }
    }

    public void Run()
    {
        Task.Run(() => RunCore());
    }

    private void RunCore()
    {
        while (true)
        {
            bool tileInfoRequired = false;

            lock (harvestLock)
            {
                if (disposed)
                    return;

                while (fileOutputQueue.Count > 0)
                {
                    var output = fileOutputQueue.Dequeue();
                    if (output.Equals("harvest:"))
                    {
                        tileInfoRequired = true;
                        break;
                    }

                    writer.WriteLine(output);
                    Program.Print(output);
                }
            }

            if (tileInfoRequired)
            {
                string tileInfo = Info();
                writer.WriteLine($"harvest: {tileInfo}");
                Program.Print($"harvest: {tileInfo}");
            }

            Thread.Yield();
        }
    }

    private void EnqueueOutput(string output)
    {
        lock (harvestLock)
        {
            fileOutputQueue.Enqueue(output);
        }
    }

    private void HandleTraceBuildingCommand(object sender, string e)
    {
        switch (e)
        {
            case "harvest":
                EnqueueOutput("harvest:");
                break;
            case "walk":
                EnqueueOutput($"walk: {Me.Location}");
                break;
            default:
                Program.Print($"Unhandled command: {e}");
                break;
        }

    }
}