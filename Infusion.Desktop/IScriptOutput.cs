namespace Infusion.Desktop
{
    public interface IScriptOutput
    {
        void Echo(string text);
        void Error(string text);
        void Result(string text);
        void Info(string text);
        void Debug(string text);
    }
}