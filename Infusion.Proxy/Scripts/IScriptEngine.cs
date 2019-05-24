using System.Threading;
using System.Threading.Tasks;

namespace Infusion.Proxy.Scripts
{
    public interface IScriptEngine
    {
        string ScriptRootPath { get; set; }
        void Reset();
        Task ExecuteScript(string scriptPath, CancellationTokenSource cancellationTokenSource);
    }
}
