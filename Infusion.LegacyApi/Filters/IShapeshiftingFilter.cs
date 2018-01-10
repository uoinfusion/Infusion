namespace Infusion.LegacyApi.Filters
{
    public interface IShapeshiftingFilter
    {
        void AddShapeShift(ItemSpec spec, ModelId targetType, Color? targetColor = null);
        void Disable();
        void Enable();
        void Reset();
    }
}