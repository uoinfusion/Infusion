using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Infusion.Packets;
using Infusion.Packets.Both;
using Infusion.Packets.Client;

namespace Infusion.Proxy.LegacyApi
{
    public class Player
    {
        private const int MaxEnqueuedWalkRequests = 0;
        private static readonly ModelId BackPackType = (ModelId) 0x0E75;

        private static readonly TimeSpan TimeBetweenRunningSteps = TimeSpan.FromMilliseconds(190);
        private static readonly TimeSpan TimeBetweenWalkingSteps = TimeSpan.FromMilliseconds(400);

        private readonly AutoResetEvent walkRequestDequeueEvent = new AutoResetEvent(false);

        public uint PlayerId { get; set; }

        private Location3D location;

        public Location3D Location
        {
            get { return location; }
            set
            {
                if (location != value)
                {
                    location = value;
                    OnLocationChanged(value);
                }
            }
        }

        public event EventHandler<Location3D> LocationChanged;

        public Location3D PredictedLocation { get; set; }
        public Movement PredictedMovement { get; set; }

        public Movement Movement { get; set; }
        internal byte CurrentSequenceKey { get; set; }
        internal WalkRequestQueue WalkRequestQueue { get; } = new WalkRequestQueue();

        public Item BackPack => Injection.Items.FirstOrDefault(i => i.Type == BackPackType && i.ContainerId == PlayerId);
        public Item BankBox => Injection.Items.OnLayer(Layer.BankBox).FirstOrDefault();

        public Color Color { get; set; }
        public ModelId BodyType { get; set; }
        public ushort CurrentHealth { get; set; }
        public ushort MaxHealth { get; set; }
        public ushort CurrentStamina { get; set; }
        public ushort MaxStamina { get; set; }
        public ushort Weight { get; set; }
        public ushort Dexterity { get; set; }
        public ushort Strength { get; set; }
        public ushort Intelligence { get; set; }

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
                    // TODO: time for stepping
                    timeBetweenSteps = TimeBetweenWalkingSteps;
                    break;
                case MovementType.Run:
                    timeBetweenSteps = TimeBetweenRunningSteps;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(movementType), $"Unknown MovementType {movementType}");
            }

            if (lastEnqueueTime < timeBetweenSteps)
            {
                var waitTime = timeBetweenSteps - lastEnqueueTime;
                Program.Diagnostic.Debug($"WaitToAvoidFastWalk: waiting minimal time between steps {timeBetweenSteps} - {lastEnqueueTime} = {waitTime}");
                Injection.Wait(waitTime.Milliseconds);
            }
        }

        internal void WaitWalkAcknowledged()
        {
            Program.Diagnostic.Debug($"WaitWalkAcknowledged: WalkRequestQueue.Count = {WalkRequestQueue.Count}");

            while (WalkRequestQueue.Count > MaxEnqueuedWalkRequests)
            {
                Program.Diagnostic.Debug($"WaitWalkAcknowledged: too many walk WalkRequestQueue.Count = {WalkRequestQueue.Count}");
                Injection.CheckCancellation();
                walkRequestDequeueEvent.WaitOne(200);
            }
        }

        internal void OnWalkRequestDequeued(object sender, EventArgs e)
        {
            walkRequestDequeueEvent.Set();
        }

        internal void Walk(Direction direction, MovementType movementType)
        {
            Program.Diagnostic.Debug($"Walk: direction = {direction}, movementType = {movementType}");
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
            Program.Diagnostic.Debug(
                $"Walk: WalkRequest enqueued, Direction = {direction}, usedSequenceKey={packet.SequenceKey}, currentSequenceKey = {CurrentSequenceKey}, queue length = {WalkRequestQueue.Count}");

            Program.SendToServer(packet.RawPacket);
        }

        private void OnLocationChanged(Location3D e)
        {
            LocationChanged?.Invoke(this, e);
        }

        public ImmutableDictionary<Skill, SkillValue> Skills { get; private set; } = ImmutableDictionary<Skill, SkillValue>.Empty;

        public void UpdateSkills(IEnumerable<SkillValue> skillValues)
        {
            Skills = Skills.SetItems(skillValues.Select(x => new KeyValuePair<Skill, SkillValue>(x.Skill, x)));
        }
    }
}