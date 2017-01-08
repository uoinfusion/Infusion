using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using UltimaRX.Packets;

namespace UltimaRX.Proxy.InjectionApi
{
    public class Player
    {
        public uint PlayerId { get; set; }

        public Location3D Location { get; set; }
        public Direction Direction { get; set; }
        internal byte CurrentSequenceKey { get; set; }

        private ConcurrentQueue<WalkRequest> walkRequestQueue = new ConcurrentQueue<WalkRequest>();
        internal ConcurrentQueue<WalkRequest> WalkRequestQueue => walkRequestQueue;

        public Item BackPack => Injection.Items.First(i => i.Type == 0x0E75 && i.ContainerId == PlayerId);

        internal void ResetWalkRequestQueue()
        {
            var newQueue = new ConcurrentQueue<WalkRequest>();
            Interlocked.Exchange(ref walkRequestQueue, newQueue);
        }
    }
}