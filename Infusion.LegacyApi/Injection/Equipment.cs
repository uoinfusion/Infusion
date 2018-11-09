using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Injection
{
    internal struct Equipment
    {
        public ObjectId Id { get; }
        public Layer Layer { get; }

        public Equipment(ObjectId id, Layer layer)
        {
            Id = id;
            Layer = layer;
        }
    }
}
