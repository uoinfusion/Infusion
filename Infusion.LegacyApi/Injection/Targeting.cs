namespace Infusion.LegacyApi.Injection
{
    internal sealed class Targeting
    {
        private readonly Legacy api;
        private readonly InjectionHost host;

        public Targeting(Legacy api, InjectionHost host)
        {
            this.api = api;
            this.host = host;
        }

        public void WaitTargetObject(ObjectId id)
        {
            api.WaitTargetObject(id);
        }

        public void WaitTargetObject(string idText)
        {
            var id = host.GetObject(idText);
            api.WaitTargetObject((uint)id);
        }

        public void WaitTargetObject(string idText1, string idText2)
        {
            var id1 = host.GetObject(idText1);
            var id2 = host.GetObject(idText2);

            api.WaitTargetObject((uint)id1, (uint)id2);
        }
    }
}
