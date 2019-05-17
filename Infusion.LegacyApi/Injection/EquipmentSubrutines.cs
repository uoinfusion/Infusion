using System;
using System.Collections.Generic;

namespace Infusion.LegacyApi.Injection
{
    internal sealed class EquipmentSubrutines
    {
        private readonly Legacy api;

        public EquipmentSubrutines(Legacy api)
        {
            this.api = api;
        }

        public void Equip(int layer, int id)
        {
            var item = api.Items[(ObjectId)id];
            if (item != null)
            {
                api.DragItem(item.Id);
                api.Wear(item.Id, (Layer)layer);
            }
        }

        public void Unequip(int layer)
        {
            var item = api.Items.OnLayer((Layer)layer).FirstOrDefault();

            if (item != null)
            {
                api.DragItem(item.Id);
                api.DropItem(item, api.Me.BackPack);
            }
        }

        public int ObjAtLayer(int layer)
        {
            var item = UO.Items.OnLayer((Layer)layer).FirstOrDefault();
            if (item == null)
                return 0;

            return (int)item.Id;
        }
    }
}
