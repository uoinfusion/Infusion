using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UltimaRX.Proxy;
using static UltimaRX.Proxy.Program;

public class HarvestMapBuilder : IDisposable
{
    private readonly Stream file;

    private readonly Queue<string> fileOutputQueue = new Queue<string>();
    private readonly object harvestLock = new object();
    private readonly StreamWriter writer;
    private bool disposed;
    private bool infoRequired;

    public HarvestMapBuilder(string fileName)
    {
        file = File.Create(fileName);
        writer = new StreamWriter(file);
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
            bool treeInfoRequired = false;

            lock (harvestLock)
            {
                if (disposed)
                    return;

                while (fileOutputQueue.Count > 0)
                {
                    var output = fileOutputQueue.Dequeue();
                    if (output.Equals("tree:"))
                    {
                        treeInfoRequired = true;
                        break;
                    }

                    writer.WriteLine(output);
                }
            }

            if (treeInfoRequired)
            {
                string treeInfo = Info();
                writer.WriteLine($"tree: {treeInfo}");
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
            case "tree":
                EnqueueOutput("tree:");
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