using Infusion.Commands;
using Infusion.LegacyApi.Console;
using InjectionScript.Debugging;
using InjectionScript.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Injection
{
    internal sealed class DebuggerBridge
    {
        private readonly CommandHandler commandHandler;
        private readonly DebuggerServer debuggerServer;
        private readonly IConsole console;
        private readonly InjectionRuntime runtime;

        public DebuggerBridge(CommandHandler commandHandler, DebuggerServer debugerServer, IConsole console, InjectionRuntime runtime)
        {
            this.commandHandler = commandHandler;
            this.debuggerServer = debugerServer;
            this.console = console;
            this.runtime = runtime;
        }

        public void RegisterCommands()
        {
            commandHandler.RegisterCommand(new Command("dbg-continue", debuggerServer.Continue, terminable: false, builtIn: true));
            commandHandler.RegisterCommand(new Command("dbg-breakpoint-add", AddBreakpoint, terminable: false, builtIn: true));
            commandHandler.RegisterCommand(new Command("dbg-breakpoint-remove", RemoveBreakpoint, terminable: false, builtIn: true));
            commandHandler.RegisterCommand(new Command("dbg-breakpoint-list", ListBreakpoints, terminable: false, builtIn: true));
            commandHandler.RegisterCommand(new Command("dbg-eval", Evaluate, terminable: false, builtIn: true));
            commandHandler.RegisterCommand(new Command("dbg-step", Step, terminable: false, builtIn: true));
        }

        public void HandleBreakHit(DebuggerBreak debuggerBreak)
        { 
            console.Info($"Debugger stopped at {FormatBreak(debuggerBreak)}.");
            console.Info(runtime.CurrentScript.GetLine(debuggerBreak.Location.Line - 1));
        }

        private void ListBreakpoints()
        {
            var breakpoints = debuggerServer.Breakpoints;
            if (breakpoints.Any())
            {
                foreach (var br in breakpoints)
                {
                    console.Info(FormatBreak(br));
                }
            }
            else
                console.Info("No breakpoints.");
        }

        private void RemoveBreakpoint(string lineStr)
        {
            if (int.TryParse(lineStr, out int line))
            {
                if (debuggerServer.RemoveBreakpoint(runtime.CurrentScript.FileName, line))
                    console.Info($"Breakpoint removed from {FormatBreak(line)}.");
                else
                    console.Info($"Breakpoint not found {FormatBreak(line)}");
            }
            else
                console.Error($"Wrong line number {lineStr}.");
        }

        private void AddBreakpoint(string lineStr)
        {
            if (int.TryParse(lineStr, out int line))
            {
                if (!string.IsNullOrEmpty(runtime.CurrentScript.FileName))
                {
                    debuggerServer.AddBreakpoint(runtime.CurrentScript.FileName, line);
                    console.Info($"Breakpoint added {FormatBreak(line)}.");
                }
                else
                    console.Info("Cannot add breakpoint - no script loaded.");
            }
            else
                console.Error($"Wrong line number {lineStr}.");
        }

        private void Evaluate(string expr)
        {
            var result = debuggerServer.EvaluateExpression(expr);
            if (result.Messages.Any())
                console.Info(result.Messages.ToString());
            console.Info($"{expr} -> {result.Result}");
        }

        private void Step()
        {
            debuggerServer.Step();
        }

        private string FormatBreak(int line) => FormatBreak(runtime.CurrentScript.FileName, line);
        private string FormatBreak(Breakpoint br) => FormatBreak(br.FileName, br.Line);
        private string FormatBreak(DebuggerBreak br) => FormatBreak(br.Location.FileName, br.Location.Line);

        private string FormatBreak(string fileName, int line)
            => $"line: {line}, {Path.GetFileName(fileName)}";
    }
}
