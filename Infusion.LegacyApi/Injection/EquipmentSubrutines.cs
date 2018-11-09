using System.Collections.Generic;

namespace Infusion.LegacyApi.Injection
{
    internal sealed class EquipmentSubrutines
    {
        private readonly Dictionary<string, ArmSet> armSets = new Dictionary<string, ArmSet>();
        private readonly Legacy api;

        public EquipmentSubrutines(Legacy api)
        {
            this.api = api;
        }

        public void SetArm(string name)
        {
            var equipments = new List<Equipment>();

            var oneHanded = api.Items.OnLayer(Layer.OneHandedWeapon).FirstOrDefault();
            var twoHanded = api.Items.OnLayer(Layer.TwoHandedWeapon).FirstOrDefault();

            if (oneHanded != null)
                equipments.Add(new Equipment(oneHanded.Id, Layer.OneHandedWeapon));
            if (twoHanded != null)
                equipments.Add(new Equipment(twoHanded.Id, Layer.TwoHandedWeapon));

            armSets[name] = new ArmSet(api, equipments.ToArray());
        }

        public void Arm(string name)
        {
            if (armSets.TryGetValue(name, out var armSet))
            {
                armSet.Arm();
            }
            else
                api.ClientPrint($"No weapons set with setarm for {name}");

        }
    }
}
