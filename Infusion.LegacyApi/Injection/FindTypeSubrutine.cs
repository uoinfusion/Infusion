using InjectionScript.Runtime;
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
        private readonly HashSet<ObjectId> ignoredIds = new HashSet<ObjectId>();
        public int count;

        public int FindItem { get; internal set; }
        public int FindCount => count;

        public int Distance { get; internal set; } = -1;

        public FindTypeSubrutine(Legacy api, InjectionHost host)
        {
            this.api = api;
        }

        public int Count(int type) => UO.Items.OfType((ModelId)type).InBackPack().Sum(x => x.Amount);

        public void FindType(int type, int color, int container)
        {
            IEnumerable<Item> foundItems = Array.Empty<Item>();

            if (container == 1)
            {
                foundItems = UO.Items.Where(x => !ignoredIds.Contains(x.Id)).OnGround();
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

        public void Ignore(int id) => ignoredIds.Add((uint)id);
        public void IgnoreReset() => ignoredIds.Clear();
    }
}
