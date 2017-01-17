using System;
using System.Linq;
using System.Threading;
using UltimaRX.Gumps;
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
        private static readonly GumpObservers GumpObservers;

        private static readonly ThreadLocal<CancellationToken?> cancellationToken =
            new ThreadLocal<CancellationToken?>(() => null);

        private static readonly Targeting Targeting;

        static Injection()
        {
            GumpObservers = new GumpObservers(Program.ServerPacketHandler);
            Items = new ItemCollection(Me);
            ItemsObserver = new ItemsObservers(Items, Program.ServerPacketHandler);
            Me.LocationChanged += ItemsObserver.OnPlayerPositionChanged;
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

        public static ItemCollection Items { get; }

        public static Player Me { get; } = new Player();

        public static Gump WaitForGump() => GumpObservers.WaitForGump();

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
            UseType((ModelId) type);
        }

        public static void UseType(ModelId type)
        {
            CheckCancellation();

            var item = Items.OfType(type).First();
            if (item != null)
                Use(item);
            else
                Log($"Item of type {type} not found.");
        }

        public static void UseType(params ushort[] types)
        {
            UseType(types.ToModelIds());
        }

        public static void UseType(params ModelId[] types)
        {
            CheckCancellation();

            var item = Items.OfType(types).First();
            if (item != null)
                Use(item);
            else
            {
                var typesString = types.Select(u => u.ToString()).Aggregate((l, r) => l + ", " + r);

                Log($"Item of any type {typesString} not found.");
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

        public static void Run(Action scriptAction) => Script.Create(scriptAction)();

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

            var refreshedItem = Items.RefreshItem(item);
            if (refreshedItem == null)
            {
                Log($"Cannot pickup item {item.Type}, it disappeared.");
                return;
            }
            item = refreshedItem;

            var pickupPacket = new PickupItemRequest(item.Id, item.Amount);
            Program.SendToServer(pickupPacket.RawPacket);
            Wait(1000);

            var dropPacket = new DropItemRequest(item.Id, Me.BackPack.Id);
            Program.SendToServer(dropPacket.RawPacket);
        }

        public static void PickupFromGround(ushort type)
        {
            PickupFromGround((ModelId) type);
        }

        public static void PickupFromGround(params ModelId[] type)
        {
            CheckCancellation();

            var itemsOnGround = Items.OfType(type).OnGround();
            foreach (var item in itemsOnGround)
            {
                Log($"Picking up {item.Type}");
                Pickup(item);
                Wait(1000);
            }
        }

        public static void Log(string message)
        {
            Program.Print(message);
        }

        public static void SelectGumpButton(string buttonLabel)
        {
            GumpObservers.SelectGumpButton(buttonLabel);
        }
    }
}