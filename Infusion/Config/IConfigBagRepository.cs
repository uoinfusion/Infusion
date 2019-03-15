namespace Infusion.Config
{
    public interface IConfigBagRepository
    {
        T Get<T>(string name);
        void Update(string name, object value);
    }
}
