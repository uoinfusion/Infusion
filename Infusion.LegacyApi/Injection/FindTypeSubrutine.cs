using InjectionScript.Interpretation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Injection
{
    internal sealed class FindTypeSubrutine
    {
        private readonly Legacy api;
        private readonly Runtime runtime;

        public int Quantity { get; private set; }
        public int FindItem { get; internal set; }

        public int count;

        public int FindCount() => count;

        public FindTypeSubrutine(Legacy api, Runtime runtime)
        {
            this.api = api;
            this.runtime = runtime;
        }
        
        private int ConvertContainer(string id)
        {
            if (id.Equals("my", StringComparison.OrdinalIgnoreCase))
                return -1;
            else if (id.Equals("ground", StringComparison.OrdinalIgnoreCase))
                return 1;

            return runtime.GetObject(id);
        }

        public void FindType(string typeStr) => FindType(NumberConversions.Str2Int(typeStr));
        public void FindType(int type) => FindType(type, -1, -1);
        public void FindType(string type, string color, string container)
            => FindType(NumberConversions.Str2Int(type), NumberConversions.Str2Int(color), ConvertContainer(container));
        internal void FindType(int type, int color, string container)
            => FindType(type, color, ConvertContainer(container));


        public void FindType(int type, int color, int container)
        {
            Item[] foundItems = Array.Empty<Item>();

            if (container == 1)
            {
                if (color >= 0)
                    foundItems = api.Items.OfType((ModelId)type).OfColor((Color)color).OnGround().OrderByDistance().ToArray();
                else
                    foundItems = api.Items.OfType((ModelId)type).OnGround().OrderByDistance().ToArray();
            }
            else if (container == -1)
            {
                if (color >= 0)
                    foundItems = api.Items.OfType((ModelId)type).OfColor((Color)color).InBackPack(false).ToArray();
                else
                    foundItems = api.Items.OfType((ModelId)type).InBackPack(false).ToArray();
            }
            else if (container > 1)
            {
                if (color >= 0)
                    foundItems = api.Items.OfType((ModelId)type).OfColor((Color)color).InContainer((uint)container, false).ToArray();
                else
                    foundItems = api.Items.OfType((ModelId)type).InContainer((uint)container, false).ToArray();
            }

            if (foundItems.Length > 0)
            {
                Quantity = foundItems[0].Amount;
                count = foundItems.Length;
                FindItem = (int)foundItems[0].Id.Value;
            }
            else
            {
                Quantity = 0;
                count = 0;
                FindItem = 0;
            }
        }
    }
}
