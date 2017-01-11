using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using UltimaRX.Packets;

namespace UltimaRX.Proxy.InjectionApi
{
    public class Player
    {
        private static readonly ModelId BackPackType = (ModelId) 0x0E75;

        private ConcurrentQueue<WalkRequest> walkRequestQueue = new ConcurrentQueue<WalkRequest>();
        public uint PlayerId { get; set; }

        public Location3D Location { get; set; }
        public Movement Movement { get; set; }
        internal byte CurrentSequenceKey { get; set; }
        internal ConcurrentQueue<WalkRequest> WalkRequestQueue => walkRequestQueue;

        public Item BackPack => Injection.Items.First(i => i.Type == BackPackType && i.ContainerId == PlayerId);
        public Color Color { get; set; }
        public ModelId BodyType { get; set; }
        public ushort CurrentHealth { get; set; }
        public ushort MaxHealth { get; set; }

        internal void ResetWalkRequestQueue()
        {
            var newQueue = new ConcurrentQueue<WalkRequest>();
            Interlocked.Exchange(ref walkRequestQueue, newQueue);
        }
    }
}