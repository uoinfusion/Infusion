using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Infusion.Commands;
using Infusion.EngineScripts;
using Infusion.LegacyApi;
using Infusion.LegacyApi.Console;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Text;

namespace Infusion.Desktop.Scripts
{
    public class RoslynPadScriptEngineBridge : RoslynPad.UI.IScriptEngine
    {
        private readonly CSharpScriptEngine csharpScriptEngine;

        public RoslynPadScriptEngineBridge(CSharpScriptEngine csharpScriptEngine)
        {
            this.csharpScriptEngine = csharpScriptEngine;
        }

        public Task<object> Execute(string code, string filePath, bool wholeFile, CancellationTokenSource cancellationTokenSource = null) 
            => csharpScriptEngine.Execute(code, filePath, wholeFile, cancellationTokenSource);
    }
}

