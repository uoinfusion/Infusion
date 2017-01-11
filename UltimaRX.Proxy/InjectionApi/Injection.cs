using System;
using System.Linq;
using System.Threading;
using UltimaRX.Packets;
using UltimaRX.Packets.Both;
using UltimaRX.Packets.Client;

namespace UltimaRX.Proxy.InjectionApi
{
    public static class Injection
    {
        private static readonly ItemsObservers ItemsObserver;
        private static readonly Journal Journal;
        private static readonly JournalObservers JournalObservers;
        private static readonly PlayerObservers PlayerObservers;
        private static readonly BlockedPacketsFilters BlockedPacketsFilters;
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
            PlayerObservers.WalkRequestDequeued += Me.OnWalkRequestDequeued;
            Targeting = new Targeting(Program.ServerPacketHandler, Program.ClientPacketHandler);
            InjectionCommandHandler = new InjectionCommandHandler(Program.ClientPacketHandler);
            BlockedPacketsFilters = new BlockedPacketsFilters(Program.ServerPacketHandler);
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
            UseType((ModelId)type);
        }

        public static void UseType(ModelId type)
        {
            CheckCancellation();

            var item = Items.FindType(type);
            if (item != null)
                Use(item);
            else
                Program.Console.WriteLine($"Item of type {type} not found.");
        }

        public static void UseType(params ushort[] types)
        {
            UseType(types.Select(t => (ModelId) t).ToArray());
        }

        public static void UseType(params ModelId[] types)
        {
            CheckCancellation();

            var item = Items.FindType(types);
            if (item != null)
                Use(item);
            else
            {
                var typesString = types.Select(u => u.ToString()).Aggregate((l, r) => l + ", " + r);

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

        public static void WaitWalk()
        {
            Me.WaitWalk();
        }

        public static void Walk(Direction direction, MovementType movementType = MovementType.Walk)
        {
            CheckCancellation();

            Me.Walk(direction, movementType);
        }

        public static void WarModeOn()
        {
            var packet = new RequestWarMode(WarMode.Fighting);
            Program.SendToServer(packet.RawPacket);
        }

        public static void WarModeOff()
        {
            var packet = new RequestWarMode(WarMode.Normal);
            Program.SendToServer(packet.RawPacket);
        }

        public static void Attack(Item target)
        {
            var packet = new AttackRequest(target.Id);
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

        public static ModelId TypeInfo() => Targeting.TypeInfo();

        public static Item ItemInfo()
        {
            var itemId = Targeting.ItemIdInfo();

            Item item;
            if (!Items.TryGet(itemId, out item))
                return null;

            return item;
        }

        public static void WaitForTarget()
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
            PickupFromGround((ModelId)type);
        }

        public static void PickupFromGround(ModelId type)
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