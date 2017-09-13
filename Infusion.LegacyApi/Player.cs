using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Infusion.Packets.Both;
using Infusion.Packets.Server;

namespace Infusion.LegacyApi
{
    public class Player
    {
        private const int MaxEnqueuedWalkRequests = 0;
        private static readonly ModelId backPackType = 0x0E75;

        private static readonly TimeSpan timeBetweenRunningStepsOnMount = TimeSpan.FromMilliseconds(100);
        private static readonly TimeSpan timeBetweenRunningSteps = TimeSpan.FromMilliseconds(190);
        private static readonly TimeSpan timeBetweenWalkingSteps = TimeSpan.FromMilliseconds(400);
        private readonly Func<bool> hasMount;
        private readonly Legacy legacyApi;
        private readonly UltimaServer server;

        private readonly AutoResetEvent walkRequestDequeueEvent = new AutoResetEvent(false);
        private readonly object walkRequestLock = new object();

        private Location3D location;

        private int walkRequestRejectionsCount;

        internal Player(Func<bool> hasMount, UltimaServer server, Legacy legacyApi)
        {
            this.hasMount = hasMount;
            this.server = server;
            this.legacyApi = legacyApi;
        }

        public ObjectId PlayerId { get; set; }

        public Location3D Location
        {
            get => location;
            set
            {
                if (location != value)
                {
                    location = value;
                    OnLocationChanged(value);
                }
            }
        }

        internal Location3D PredictedLocation { get; set; }
        internal Direction PredictedDirection { get; set; }

        public Direction Direction { get; internal set; }
        public MovementType MovementType { get; internal set; }
        internal byte CurrentSequenceKey { get; set; }
        internal WalkRequestQueue WalkRequestQueue { get; } = new WalkRequestQueue();

        public Item BackPack => legacyApi.GameObjects.OfType<Item>().FirstOrDefault(i =>
            i.Type == backPackType && i.Layer == Layer.Backpack && i.ContainerId.HasValue && i.ContainerId == PlayerId);

        public Item BankBox => legacyApi.Items.OnLayer(Layer.BankBox).FirstOrDefault();

        public Color Color { get; internal set; }
        public ModelId BodyType { get; internal set; }
        public ushort CurrentHealth { get; set; }
        public ushort MaxHealth { get; internal set; }
        public ushort CurrentStamina { get; internal set; }
        public ushort MaxStamina { get; internal set; }
        public ushort CurrentMana { get; internal set; }
        public ushort MaxMana { get; internal set; }
        public ushort Weight { get; internal set; }
        public ushort Dexterity { get; internal set; }
        public ushort Strength { get; internal set; }
        public ushort Intelligence { get; internal set; }

        public ImmutableDictionary<Skill, SkillValue> Skills { get; private set; } =
            ImmutableDictionary<Skill, SkillValue>.Empty;

        public byte LightLevel { get; internal set; }

        public event EventHandler<Location3D> LocationChanged;

        internal void WaitToAvoidFastWalk(MovementType movementType)
        {
            var lastEnqueueTime = WalkRequestQueue.LastEnqueueTime;
            TimeSpan timeBetweenSteps;

            switch (movementType)
            {
                case MovementType.Walk:
                    timeBetweenSteps = timeBetweenWalkingSteps;
                    break;
                case MovementType.Run:
                    if (hasMount != null && hasMount())
                        timeBetweenSteps = timeBetweenRunningStepsOnMount;
                    else
                        timeBetweenSteps = timeBetweenRunningSteps;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(movementType), $"Unknown MovementType {movementType}");
            }

            if (lastEnqueueTime < timeBetweenSteps)
            {
                var waitTime = timeBetweenSteps - lastEnqueueTime;
                legacyApi.Wait(waitTime.Milliseconds);
            }
        }

        internal bool WaitWalkAcknowledged()
        {
            int originalWalkRequestRejectionsCount;

            lock (walkRequestLock)
                originalWalkRequestRejectionsCount = walkRequestRejectionsCount;

            while (WalkRequestQueue.Count > MaxEnqueuedWalkRequests)
            {
                legacyApi.CheckCancellation();
                walkRequestDequeueEvent.WaitOne(200);
            }

            return originalWalkRequestRejectionsCount == walkRequestRejectionsCount;
        }

        internal void OnWalkRequestDequeued(object sender, EventArgs e)
        {
            walkRequestDequeueEvent.Set();
        }

        internal void Walk(Direction direction, MovementType movementType)
        {
            WalkRequestQueue.Enqueue(new WalkRequest(CurrentSequenceKey, direction, MovementType, true));
            if (PredictedDirection != direction)
                PredictedDirection = direction;
            else
                PredictedLocation = PredictedLocation.LocationInDirection(direction);

            server.Move(direction, movementType, CurrentSequenceKey);

            CurrentSequenceKey++;
        }

        private void OnLocationChanged(Location3D e)
        {
            LocationChanged?.Invoke(this, e);
        }

        internal void UpdateSkills(IEnumerable<SkillValue> skillValues)
        {
            Skills = Skills.SetItems(skillValues.Select(x => new KeyValuePair<Skill, SkillValue>(x.Skill, x)));
        }

        public ushort GetDistance(Location3D location)
            => Location.GetDistance(location);

        public ushort GetDistance(Location2D location)
            => Location.GetDistance((Location3D) location);

        public ushort GetDistance(GameObject obj)
            => GetDistance(obj.Location);

        internal void RejectWalkRequest(CharMoveRejectionPacket packet)
        {
            lock (walkRequestLock)
            {
                Location = packet.Location;
                PredictedLocation = packet.Location;
                Direction = packet.Direction;
                MovementType = packet.MovementType;
                PredictedDirection = Direction;
                CurrentSequenceKey = 0;
                walkRequestRejectionsCount++;
                WalkRequestQueue.Reset();
                walkRequestDequeueEvent.Set();
            }
        }

        public void ResetWalkRequestQueue()
        {
            WalkRequestQueue.Reset();
        }
    }
}