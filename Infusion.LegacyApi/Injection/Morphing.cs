using InjectionScript.Interpretation;

namespace Infusion.LegacyApi.Injection
{
    internal class Morphing
    {
        private readonly Legacy api;
        private ModelId? orignalModel;
        private bool morphed = false;

        public Morphing(Legacy api) => this.api = api;

        public void Morph(string type) => Morph(NumberConversions.Str2Int(type));

        public void Morph(int type)
        {
            ModelId model = (ushort)type;

            if (!morphed)
                orignalModel = api.Me.BodyType;

            if (type <= 0)
            {
                if (orignalModel.HasValue)
                    SetPlayerModel(orignalModel.Value);
                morphed = false;
            }
            else
            {
                SetPlayerModel(model);
                morphed = true;
            }
        }

        private void SetPlayerModel(ModelId model)
            => api.Client.DrawGamePlayer(api.Me.PlayerId, model, api.Me.Location, api.Me.Direction, api.Me.MovementType, api.Me.Color);
    }
}
