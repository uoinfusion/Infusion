namespace Infusion.LegacyApi.Filters
{
    public interface IStaminaFilter
    {
        void SetFakeStamina(ushort stamina);
        void Disable();
    }
}