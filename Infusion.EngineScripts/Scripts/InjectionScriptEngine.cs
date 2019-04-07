using Infusion.LegacyApi.Console;
using Infusion.LegacyApi.Injection;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Infusion.EngineScripts
{
    public class InjectionScriptEngine : IScriptEngine
    {
        private readonly InjectionHost injection;
        private readonly IConsole scriptOutput;
        private string currentFileName;
        private readonly FileSystemWatcher fileWatcher = new FileSystemWatcher();
        private bool fileChanged;
        private readonly System.Timers.Timer timer;
        private readonly object currentFileLock = new object();

        public InjectionScriptEngine(InjectionHost injection, IConsole scriptOutput)
        {
            this.injection = injection;
            this.scriptOutput = scriptOutput;
            this.fileWatcher.Changed += ScriptFileChanged;
            scriptOutput.WriteLine(ConsoleLineType.Important, currentFileName);
            timer = new System.Timers.Timer(1000) { AutoReset = true };
            timer.Elapsed += (timerElapsedSender, timerElapsedArgs) =>
            {
                lock (currentFileLock)
                {
                    if (fileChanged)
                    {
                        timer.Stop();
                        try
                        {
                            ExecuteScript(currentFileName, new CancellationTokenSource()).Wait();
                        }
                        finally
                        {
                            timer.Start();
                            fileChanged = false;
                        }
                    }
                }
            };
            timer.Start();
        }

        private void ScriptFileChanged(object sender, FileSystemEventArgs e)
        {
            lock (currentFileLock)
            {
                fileChanged = true;
            }
        }

        public string ScriptRootPath { get; set; }

        public Task ExecuteScript(string scriptPath, CancellationTokenSource cancellationTokenSource)
        {
            if (scriptPath.EndsWith(".sc", StringComparison.OrdinalIgnoreCase))
            {
                return Task.Run(() =>
                {
                    lock (currentFileLock)
                    {
                        try
                        {
                            var watch = Stopwatch.StartNew();
                            injection.LoadScript(scriptPath);
                            Directory.SetCurrentDirectory(Path.GetDirectoryName(scriptPath));
                            watch.Stop();

                            scriptOutput.Info($"Loaded in {watch.ElapsedMilliseconds} ms.");

                            if (!scriptPath.Equals(currentFileName, StringComparison.OrdinalIgnoreCase))
                            {
                                currentFileName = scriptPath;
                                fileWatcher.Path = Path.GetDirectoryName(scriptPath);
                                fileWatcher.Filter = Path.GetFileName(scriptPath);
                                fileWatcher.NotifyFilter = NotifyFilters.LastWrite;
                            }
                            fileWatcher.EnableRaisingEvents = true;
                        }
                        catch (Exception ex)
                        {
                            scriptOutput.Error(ex.ToString());
                        }
                    }
                });
            }
            else
                return Task.CompletedTask;
        }

        public void Reset() { }
    }
}
