using System;
using System.Linq;
using System.Threading;
using UltimaRX.Packets;
using UltimaRX.Packets.Client;

namespace UltimaRX.Proxy.InjectionApi
{
    public class Player
    {
        private const int MaxEnqueuedWalkRequests = 1;
        private static readonly ModelId BackPackType = (ModelId) 0x0E75;

        private static readonly TimeSpan TimeBetweenSteps = TimeSpan.FromMilliseconds(190);

        private readonly AutoResetEvent walkRequestDequeueEvent = new AutoResetEvent(false);

        public uint PlayerId { get; set; }

        private Location3D location;

        public Location3D Location
        {
            get { return location; }
            set
            {
                location = value;
                OnLocationChanged(value);
            }
        }

        public event EventHandler<Location3D> LocationChanged;

        public Location3D PredictedLocation { get; set; }
        public Movement PredictedMovement { get; set; }

        public Movement Movement { get; set; }
        internal byte CurrentSequenceKey { get; set; }
        internal WalkRequestQueue WalkRequestQueue { get; } = new WalkRequestQueue();

        public Item BackPack => Injection.Items.First(i => i.Type == BackPackType && i.ContainerId == PlayerId);
        public Color Color { get; set; }
        public ModelId BodyType { get; set; }
        public ushort CurrentHealth { get; set; }
        public ushort MaxHealth { get; set; }

        internal void ResetWalkRequestQueue()
        {
            WalkRequestQueue.Reset();
        }

        internal void WaitWalk()
        {
            var timeSinceLastEnqueue = WalkRequestQueue.TimeSinceLastEnqueue;
            if (timeSinceLastEnqueue < TimeBetweenSteps)
            {
                var waitTime = TimeBetweenSteps - timeSinceLastEnqueue;
                Program.Diagnostic.WriteLine($"Walk: waiting minimal time between steps {waitTime}");
                Injection.Wait(waitTime.Milliseconds);
            }

            while (WalkRequestQueue.Count > MaxEnqueuedWalkRequests)
            {
                Program.Diagnostic.WriteLine("Walk: too many walk requests");
                Injection.CheckCancellation();
                walkRequestDequeueEvent.WaitOne(1000);
            }
        }

        internal void OnWalkRequestDequeued(object sender, EventArgs e)
        {
            walkRequestDequeueEvent.Set();
        }

        internal void Walk(Direction direction, MovementType movementType)
        {
            var packet = new MoveRequest
            {
                Movement = new Movement(direction, movementType),
                SequenceKey = CurrentSequenceKey
            };

            WalkRequestQueue.Enqueue(new WalkRequest(CurrentSequenceKey, packet.Movement, true));
            if (PredictedMovement.Direction != packet.Movement.Direction)
            {
                PredictedMovement = packet.Movement;
            }
            else
            {
                PredictedLocation = PredictedLocation.LocationInDirection(direction);
            }

            CurrentSequenceKey++;
            Program.Diagnostic.WriteLine(
                $"Walk: WalkRequest enqueued, Direction = {direction}, usedSequenceKey={packet.SequenceKey}, currentSequenceKey = {CurrentSequenceKey}, queue length = {WalkRequestQueue.Count}");

            Program.SendToServer(packet.RawPacket);
        }

        private void OnLocationChanged(Location3D e)
        {
            LocationChanged?.Invoke(this, e);
        }
    }
}