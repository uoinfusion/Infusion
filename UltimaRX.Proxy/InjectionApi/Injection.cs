using System;
using System.Linq;
using System.Threading;
using UltimaRX.Packets;
using UltimaRX.Packets.Client;

namespace UltimaRX.Proxy.InjectionApi
{
    public static class Injection
    {
        private static readonly ItemsObservers ItemsObserver;
        private static readonly Journal Journal;
        private static readonly JournalObservers JournalObservers;
        private static readonly PlayerObservers PlayerObservers;
        private static readonly InjectionCommandHandler InjectionCommandHandler;

        private static readonly ThreadLocal<CancellationToken?> cancellationToken =
            new ThreadLocal<CancellationToken?>(() => null);

        private static readonly Targeting Targeting;

        static Injection()
        {
            ItemsObserver = new ItemsObservers(Items, Program.ServerPacketHandler);
            Journal = new Journal();
            JournalObservers = new JournalObservers(Journal, Program.ServerPacketHandler);
            PlayerObservers = new PlayerObservers(Me, Program.ClientPacketHandler, Program.ServerPacketHandler);
            Targeting = new Targeting(Program.ServerPacketHandler, Program.ClientPacketHandler);
            InjectionCommandHandler = new InjectionCommandHandler(Program.ClientPacketHandler);
        }

        internal static CancellationToken? CancellationToken
        {
            get { return cancellationToken.Value; }
            set { cancellationToken.Value = value; }
        }

        public static ItemCollection Items { get; } = new ItemCollection();

        public static Player Me { get; } = new Player();

        public static event EventHandler<string> CommandReceived
        {
            add { InjectionCommandHandler.CommandReceived += value; }
            remove { InjectionCommandHandler.CommandReceived -= value; }
        }

        public static void Initialize()
        {
        }

        public static void Use(uint objectId)
        {
            CheckCancellation();

            var packet = new DoubleClickRequest(objectId);
            Program.SendToServer(packet.RawPacket);
        }

        internal static void CheckCancellation()
        {
            cancellationToken.Value?.ThrowIfCancellationRequested();
        }

        public static void Use(Item item)
        {
            Use(item.Id);
        }

        public static void UseType(ushort type)
        {
            CheckCancellation();

            var item = Items.FindType(type);
            if (item != null)
                Use(item);
            else
                Program.Console.WriteLine($"Item of type {type:X4} not found.");
        }

        public static void UseType(params ushort[] types)
        {
            CheckCancellation();

            var item = Items.FindType(types);
            if (item != null)
                Use(item);
            else
            {
                var typesString = types.Select(u => u.ToString("X4")).Aggregate((l, r) => l + ", " + r);

                Program.Console.WriteLine($"Item of any type {typesString} not found.");
            }
        }

        public static bool InJournal(params string[] words) => Journal.InJournal(words);

        public static void DeleteJournal()
        {
            CheckCancellation();

            Journal.DeleteJournal();
        }

        public static void WaitForJournal(params string[] words)
        {
            CheckCancellation();

            Journal.WaitForJournal(words);
        }

        public static void Wait(int milliseconds)
        {
            CheckCancellation();

            Thread.Sleep(milliseconds);
        }

        public static void Walk(Direction direction)
        {
            CheckCancellation();

            var packet = new MoveRequest
            {
                Movement = new Movement(direction, MovementType.Walk),
                SequenceKey = Me.CurrentSequenceKey
            };

            Me.WalkRequestQueue.Enqueue(new WalkRequest(Me.CurrentSequenceKey, direction, true));

            Me.CurrentSequenceKey++;
            Program.Diagnostic.WriteLine(
                $"Walk: WalkRequest enqueued, Direction = {direction}, usedSequenceKey={packet.SequenceKey}, currentSequenceKey = {Me.CurrentSequenceKey}, queue length = {Me.WalkRequestQueue.Count}");

            Program.SendToServer(packet.RawPacket);
        }

        public static void TargetTile(string tileInfo)
        {
            CheckCancellation();

            Targeting.TargetTile(tileInfo);
        }

        public static void Target(Item item)
        {
            CheckCancellation();

            Targeting.Target(item);
        }

        public static Script Run(Action scriptAction) => Script.Run(scriptAction);

        public static void Terminate()
        {
            Script.Terminate();
        }

        public static string Info() => Targeting.Info();

        private static void WaitForTarget()
        {
            CheckCancellation();

            Targeting.WaitForTarget();
        }

        public static void Pickup(Item item)
        {
            CheckCancellation();

            var pickupPacket = new PickupItemRequest(item.Id, item.Amount);
            Program.SendToServer(pickupPacket.RawPacket);
            Wait(1000);

            var dropPacket = new DropItemRequest(item.Id, Me.BackPack.Id);
            Program.SendToServer(dropPacket.RawPacket);
        }

        public static void PickupFromGround(ushort type)
        {
            CheckCancellation();

            var item = Items.FindTypeOnGround(type);
            if (item != null)
            {
                Pickup(item);
            }
            else
            {
                Program.Print("No item found on ground");
            }
        }
    }
}