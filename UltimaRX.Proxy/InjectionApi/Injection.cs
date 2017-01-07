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

        static Injection()
        {
            ItemsObserver = new ItemsObservers(Items, Program.ServerPacketHandler);
            Journal = new Journal();
            JournalObservers = new JournalObservers(Journal, Program.ServerPacketHandler);
            PlayerObservers = new PlayerObservers(Me, Program.ClientPacketHandler, Program.ServerPacketHandler);
            Targeting = new Targeting(Program.ServerPacketHandler, Program.ClientPacketHandler);
            InjectionCommandHandler = new InjectionCommandHandler(Program.ClientPacketHandler);
        }

        public static event EventHandler<string> CommandReceived
        {
            add { InjectionCommandHandler.CommandReceived += value; }
            remove { InjectionCommandHandler.CommandReceived -= value; }
        }

        public static ItemCollection Items { get; } = new ItemCollection();

        public static Player Me { get; } = new Player();

        public static void Initialize()
        {
        }

        public static void Use(int objectId)
        {
            var packet = new DoubleClickRequest(objectId);
            Program.SendToServer(packet.RawPacket);
        }

        public static void Use(Item item)
        {
            Use(item.Id);
        }

        public static void UseType(ushort type)
        {
            var item = Items.FindType(type);
            if (item != null)
                Use(item);
            else
                Program.Console.WriteLine($"Item of type {type:X4} not found.");
        }

        public static void UseType(params ushort[] types)
        {
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
            Journal.DeleteJournal();
        }

        public static void WaitForJournal(params string[] words)
        {
            Journal.WaitForJournal(words);
        }

        public static void Wait(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

        public static void Walk(Direction direction)
        {
            var packet = new MoveRequest
            {
                Movement = new Movement(direction, MovementType.Walk),
                SequenceKey = Me.CurrentSequenceKey
            };

            Me.WalkRequestQueue.Enqueue(new WalkRequest(Me.CurrentSequenceKey, direction, true));

            Me.CurrentSequenceKey++;
            Program.Diagnostic.WriteLine($"Walk: WalkRequest enqueued, Direction = {direction}, usedSequenceKey={packet.SequenceKey}, currentSequenceKey = {Me.CurrentSequenceKey}, queue length = {Me.WalkRequestQueue.Count}");

            Program.SendToServer(packet.RawPacket);
        }

        private static readonly Targeting Targeting;


        public static void TargetTile(string tileInfo)
        {
            Targeting.TargetTile(tileInfo);
        }

        public static string Info() => Targeting.Info();

        private static void WaitForTarget()
        {
            Targeting.WaitForTarget();
        }
    }
}