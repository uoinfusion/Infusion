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
        public int count;

        public int FindItem { get; internal set; }
        public int FindCount => count;

        public int Distance { get; internal set; } = -1;

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
            IEnumerable<Item> foundItems = Array.Empty<Item>();

            if (container == 1)
            {
                foundItems = UO.Items.OnGround();
                if (Distance >= 0)
                    foundItems = foundItems.MaxDistance((ushort)Distance);
                foundItems = foundItems.OrderByDistance();
            }
            else if (container == -1)
                foundItems = UO.Items.InBackPack(false);
            else if (container > 1)
                foundItems = UO.Items.InContainer((uint)container, false);

            if (color >= 0)
                foundItems = foundItems.OfColor((Color)color);

            foundItems = foundItems.OfType((ModelId)type).ToArray();

            if (foundItems.Any())
            {
                count = foundItems.Count();
                FindItem = (int)foundItems.First().Id.Value;
            }
            else
            {
                count = 0;
                FindItem = 0;
            }
        }
    }
}
