namespace Infusion.Config
{
    public interface IConfigBagRepository
    {
        T Get<T>(string name);
        T Get<T>(string name, T defaultValue);
        void Update(string name, object value);
    }
}
