using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Infusion.Packets;
using Infusion.Packets.Both;
using Infusion.Packets.Client;

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
        private readonly UltimaServer server;
        private readonly Legacy legacyApi;

        private readonly AutoResetEvent walkRequestDequeueEvent = new AutoResetEvent(false);

        private Location3D location;

        internal Player(Func<bool> hasMount, UltimaServer server, Legacy legacyApi)
        {
            this.hasMount = hasMount;
            this.server = server;
            this.legacyApi = legacyApi;
        }

        public uint PlayerId { get; set; }

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

        public Location3D PredictedLocation { get; set; }
        public Movement PredictedMovement { get; set; }

        public Movement Movement { get; set; }
        internal byte CurrentSequenceKey { get; set; }
        internal WalkRequestQueue WalkRequestQueue { get; } = new WalkRequestQueue();

        public Item BackPack => legacyApi.GameObjects.OfType<Item>().FirstOrDefault(i => i.Type == backPackType && i.Layer == Layer.Backpack);
        public Item BankBox => legacyApi.Items.OnLayer(Layer.BankBox).FirstOrDefault();

        public Color Color { get; set; }
        public ModelId BodyType { get; set; }
        public ushort CurrentHealth { get; set; }
        public ushort MaxHealth { get; set; }
        public ushort CurrentStamina { get; set; }
        public ushort MaxStamina { get; set; }
        public ushort CurrentMana { get; set; }
        public ushort MaxMana { get; set; }
        public ushort Weight { get; set; }
        public ushort Dexterity { get; set; }
        public ushort Strength { get; set; }
        public ushort Intelligence { get; set; }

        public ImmutableDictionary<Skill, SkillValue> Skills { get; private set; } =
            ImmutableDictionary<Skill, SkillValue>.Empty;

        public event EventHandler<Location3D> LocationChanged;

        internal void ResetWalkRequestQueue()
        {
            WalkRequestQueue.Reset();
        }

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

        internal void WaitWalkAcknowledged()
        {
            while (WalkRequestQueue.Count > MaxEnqueuedWalkRequests)
            {
                legacyApi.CheckCancellation();
                walkRequestDequeueEvent.WaitOne(200);
            }
        }

        internal void OnWalkRequestDequeued(object sender, EventArgs e)
        {
            walkRequestDequeueEvent.Set();
        }

        internal void Walk(Direction direction, MovementType movementType)
        {
            var movement = new Movement(direction, movementType);

            WalkRequestQueue.Enqueue(new WalkRequest(CurrentSequenceKey, movement, true));
            if (PredictedMovement.Direction != movement.Direction)
                PredictedMovement = movement;
            else
                PredictedLocation = PredictedLocation.LocationInDirection(direction);

            server.Move(direction, movementType, CurrentSequenceKey);

            CurrentSequenceKey++;
        }

        private void OnLocationChanged(Location3D e)
        {
            LocationChanged?.Invoke(this, e);
        }

        public void UpdateSkills(IEnumerable<SkillValue> skillValues)
        {
            Skills = Skills.SetItems(skillValues.Select(x => new KeyValuePair<Skill, SkillValue>(x.Skill, x)));
        }
    }
}