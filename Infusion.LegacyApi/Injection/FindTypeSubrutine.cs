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

        public int Count(int type, int color, int containerId)
        {
            var items = FindItems(type, color, containerId, -1, false);

            return items.OfType<Item>().Sum(x => x.Amount);
        }

        private IEnumerable<GameObject> FindItems(int type, int color, int container, int range, bool recursive)
        {
            IEnumerable<GameObject> foundObjects = Array.Empty<GameObject>();

            if (container == 1)
            {
                foundObjects = UO.GameObjects.Where(x => !ignoredIds.Contains(x.Id)).OnGround();
                range = range >= 0 ? range : Distance;
                if (range >= 0)
                    foundObjects = foundObjects.MaxDistance((ushort)range);
                foundObjects = foundObjects.OrderByDistance();
            }
            else if (container == -1)
                foundObjects = UO.Items.InBackPack(recursive);
            else if (container > 1)
                foundObjects = UO.Items.InContainer((uint)container, recursive);

            if (color >= 0)
                foundObjects = foundObjects.OfColor((Color)color);

            if (type >= 0)
                foundObjects = foundObjects.OfType((ModelId)type).ToArray();

            return foundObjects;
        }


        public int FindType(int type, int color, int container, int range, bool recursive)
        {
            var foundObjects = FindItems(type, color, container, range, recursive);

            if (foundObjects.Any())
            {
                count = foundObjects.Count();
                FindItem = (int)foundObjects.First().Id.Value;
                return (int)foundObjects.First().Id.Value;
            }
            else
            {
                count = 0;
                FindItem = 0;
                return 0;
            }
        }

        public void Ignore(int id) => ignoredIds.Add((uint)id);
        public void IgnoreReset() => ignoredIds.Clear();
    }
}
