namespace Infusion.LegacyApi.Injection
{
    internal sealed class ArmSet
    {
        private readonly Legacy api;
        private readonly Equipment[] equipments;

        public ArmSet(Legacy api, params Equipment[] equipments)
        {
            this.api = api;
            this.equipments = equipments;
        }

        public void Arm()
        {
            foreach (var item in equipments)
            {
                api.DragItem(item.Id);
                api.Wear(item.Id, item.Layer);
            }
        }
    }
}
