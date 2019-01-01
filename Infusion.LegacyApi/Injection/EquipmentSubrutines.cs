using System;
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

        public void Equip(string layer, int id)
        {
            var item = api.Items[(ObjectId)id];
            if (item != null)
            {
                api.DragItem(item.Id, 1);
                api.Wear(item.Id, Convert(layer));
            }
        }

        public void Unequip(string layer)
        {
            var item = api.Items.OnLayer(Convert(layer)).FirstOrDefault();

            if (item != null)
            {
                api.DragItem(item.Id, 1);
                api.DropItem(item, api.Me.BackPack);
            }
        }

        public int ObjAtLayer(string layer)
        {
            var item = UO.Items.OnLayer(Convert(layer)).FirstOrDefault();
            if (item == null)
                return 0;

            return (int)item.Id;
        }

        private Layer Convert(string layer)
        {
            switch (layer)
            {
                case "Rhand":
                    return Layer.OneHandedWeapon;
                case "Lhand":
                    return Layer.TwoHandedWeapon;
                case "Shoes":
                    return Layer.Shoes;
                case "Pants":
                    return Layer.Pants;
                case "Shirt":
                    return Layer.Shirt;
                case "Hat":
                    return Layer.Helm;
                case "Gloves":
                    return Layer.Gloves;
                case "Ring":
                    return Layer.Ring;
                case "Neck":
                    return Layer.Neck;
                case "Hair":
                    return Layer.Hair;
                case "Waist":
                    return Layer.Waist;
                case "Torso":
                    return Layer.Torso;
                case "Brace":
                    return Layer.Bracelet;
                case "Beard":
                    return Layer.FacialHair;
                case "Ear":
                    return Layer.Earrings;
                case "Arms":
                    return Layer.Arms;
                case "Bpack":
                    return Layer.Backpack;
                case "Bank":
                    return Layer.BankBox;
                case "Rstk":
                    return Layer.RestockContainer;
                case "NRstk":
                    return Layer.NoRestockContainer;
                case "Sell":
                    return Layer.SellContainer;
                default:
                    throw new NotImplementedException($"layer {layer}");
            }
        }
    }
}
