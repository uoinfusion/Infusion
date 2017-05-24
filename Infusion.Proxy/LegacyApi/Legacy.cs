using System;
using System.Linq;
using System.Threading;
using Infusion.Gumps;
using Infusion.Packets;
using Infusion.Packets.Both;
using Infusion.Packets.Client;
using Infusion.Packets.Server;

namespace Infusion.Proxy.LegacyApi
{
    public static class Legacy
    {
        private static readonly ItemsObservers ItemsObserver;
        private static readonly JournalObservers JournalObservers;
        private static readonly PlayerObservers PlayerObservers;
        private static readonly BlockedPacketsFilters BlockedPacketsFilters;
        private static readonly CommandHandlerObservers LegacyCommandHandler;
        private static readonly GumpObservers GumpObservers;

        private static readonly ThreadLocal<CancellationToken?> cancellationToken =
            new ThreadLocal<CancellationToken?>(() => null);

        private static readonly Targeting Targeting;

        static Legacy()
        {
            GumpObservers = new GumpObservers(Program.ServerPacketHandler, Program.ClientPacketHandler);
            Items = new ItemCollection(Me);
            ItemsObserver = new ItemsObservers(Items, Program.ServerPacketHandler);
            Me.LocationChanged += ItemsObserver.OnPlayerPositionChanged;
            Journal = new JournalEntries();
            JournalObservers = new JournalObservers(Journal, Program.ServerPacketHandler);
            PlayerObservers = new PlayerObservers(Me, Program.ClientPacketHandler, Program.ServerPacketHandler);
            PlayerObservers.WalkRequestDequeued += Me.OnWalkRequestDequeued;
            Targeting = new Targeting(Program.ServerPacketHandler, Program.ClientPacketHandler);

            RegisterDefaultScripts();

            LegacyCommandHandler = new CommandHandlerObservers(Program.ClientPacketHandler, CommandHandler);
            BlockedPacketsFilters = new BlockedPacketsFilters(Program.ServerPacketHandler);
        }

        public static Configuration Configuration { get; private set; } = new Configuration();

        public static Gump CurrentGump => GumpObservers.CurrentGump;

        public static CancellationToken? CancellationToken
        {
            get => cancellationToken.Value;
            set => cancellationToken.Value = value;
        }

        public static CommandHandler CommandHandler { get; } = new CommandHandler();

        public static UltimaMap Map { get; } = new UltimaMap();

        public static ItemCollection Items { get; }

        public static Player Me { get; } = new Player();

        public static bool HitPointNotificationEnabled
        {
            get => ItemsObserver.HitPointNotificationEnabled;

            set => ItemsObserver.HitPointNotificationEnabled = value;
        }

        public static JournalEntries Journal { get; }

        public static void RegisterCommand(string name, Action commandAction)
        {
            CommandHandler.RegisterCommand(name,
                commandAction);
        }

        public static void RegisterCommand(string name, Action<string> commandAction)
        {
            CommandHandler.RegisterCommand(name,
                commandAction);
        }

        public static void Alert(string message)
        {
            Program.Console.Critical(message);
        }

        private static void RegisterDefaultScripts()
        {
            CommandHandler.RegisterCommand(new Command("terminate", Terminate,
                "Terminates all running commands and scripts.", executionMode: CommandExecutionMode.Direct));
            CommandHandler.RegisterCommand(new Command("walkto", parameters => WalkTo(parameters),
                "Walks to the specified location.", "Example: ,walkto 1234 432"));
            CommandHandler.RegisterCommand(new Command("info", InfoCommand,
                "Shows information about selected item or tile."));
            CommandHandler.RegisterCommand(new Command("help", HelpCommand, "Shows command help."));
            CommandHandler.RegisterCommand(new Command("lastgumpinfo", LastGumpInfo,
                "Shows information about the last gump dispalyed in Ultima Online client."));
        }

        private static void HelpCommand(string parameters)
        {
            Log(CommandHandler.Help(parameters));
        }

        private static void ListRunningCommands()
        {
            foreach (var command in CommandHandler.RunningCommands)
                Log(command.Name);
        }

        public static void Say(string message)
        {
            var packet = new SpeechRequest
            {
                Type = SpeechType.Normal,
                Text = message,
                Font = 0x02b2,
                Color = 0x0003,
                Language = "ENU"
            };

            Program.SendToServer(packet.RawPacket);
        }

        public static void ClientPrint(string message, string name, uint itemId, ModelId itemModel, SpeechType type,
            Color color)
        {
            var packet = new SendSpeechPacket
            {
                Id = itemId,
                Model = itemModel,
                Type = type,
                Color = color,
                Font = 0x0003,
                Name = name,
                Message = message
            };

            packet.Serialize();

            Program.SendToClient(packet.RawPacket);
        }

        public static void ClientPrintAndLog(string message)
        {
            ClientPrint(message);
            Log(message);
        }

        public static void ClientPrint(string message)
        {
            ClientPrint(message, "System", 0, (ModelId) 0, SpeechType.Normal, (Color) 0x03B2);
        }

        public static void ClientPrint(string message, string name, Item onBehalfItem)
        {
            ClientPrint(message, name, onBehalfItem.Id, onBehalfItem.Type, SpeechType.Speech, (Color) 0x0026);
        }

        public static Gump WaitForGump(TimeSpan? timeout = null)
        {
            return GumpObservers.WaitForGump(timeout);
        }

        public static void Initialize(Configuration configuration)
        {
            Configuration = configuration;
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

        public static void RequestClientStatus(uint id)
        {
            var packet = new GetClientStatusRequest(id);

            Program.SendToServer(packet.RawPacket);
        }

        public static void RequestClientStatus(Item item)
        {
            RequestClientStatus(item.Id);
        }

        public static void Use(Item item)
        {
            Use(item.Id);
        }

        public static bool UseType(ushort type)
        {
            return UseType((ModelId) type);
        }

        public static bool UseType(ModelId type)
        {
            CheckCancellation();

            var item = Items.OfType(type).InContainer(Me.BackPack).First()
                       ?? Items.OfType(type).OnLayer(Layer.OneHandedWeapon).First()
                       ?? Items.OfType(type).OnLayer(Layer.TwoHandedWeapon).First()
                       ?? Items.OfType(type).OnLayer(Layer.Backpack).First()
                       ?? Items.OfType(type).First();
            if (item != null)
                Use(item);
            else
                return false;

            return true;
        }

        public static bool UseType(params ushort[] types)
        {
            return UseType(types.ToModelIds());
        }

        public static bool UseType(params ModelId[] types)
        {
            CheckCancellation();

            var item = Items.OfType(types).InContainer(Me.BackPack).First()
                       ?? Items.OfType(types).OnLayer(Layer.OneHandedWeapon).First()
                       ?? Items.OfType(types).OnLayer(Layer.TwoHandedWeapon).First()
                       ?? Items.OfType(types).OnLayer(Layer.Backpack).First()
                       ?? Items.OfType(types).First();

            if (item != null)
            {
                Use(item);
                return true;
            }

            var typesString = types.Select(u => u.ToString()).Aggregate(string.Empty, (l, r) => l + ", " + r);
            Log($"Item of any type {typesString} not found.");

            return false;
        }

        public static bool InJournal(params string[] words)
        {
            return Journal.InJournal(words);
        }

        public static bool InJournal(DateTime createdAfter, params string[] words)
        {
            return Journal.InJournal(createdAfter, words);
        }

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
            while (milliseconds > 0)
            {
                CheckCancellation();
                Thread.Sleep(50);
                milliseconds -= 50;
            }
        }

        public static void Wait(TimeSpan span)
        {
            Wait((int) span.TotalMilliseconds);
        }

        public static void WaitToAvoidFastWalk(MovementType movementType)
        {
            Me.WaitToAvoidFastWalk(movementType);
        }

        public static void WaitWalkAcknowledged()
        {
            CheckCancellation();
            Me.WaitWalkAcknowledged();
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

        public static void Target(Player player)
        {
            CheckCancellation();

            Targeting.Target(player);
        }

        public static void Terminate(string parameters)
        {
            try
            {
                Log("Terminate attempt");
                if (string.IsNullOrEmpty(parameters))
                    CommandHandler.Terminate();
                else
                    CommandHandler.Terminate(parameters);
            }
            finally
            {
                Log("Terminate attempt finished");
            }
        }

        public static string Info()
        {
            return Targeting.Info();
        }

        private static void InfoCommand()
        {
            var info = Info();
            ClientPrintAndLog(!string.IsNullOrEmpty(info) ? info : "Targeting cancelled.");
        }

        public static ModelId TypeInfo()
        {
            return Targeting.TypeInfo();
        }

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

        public static void DropItem(Item item, Item targetContainer)
        {
            CheckCancellation();

            var dropPacket = new DropItemRequest(item.Id, targetContainer.Id);
            Program.SendToServer(dropPacket.RawPacket);
        }

        public static void DragItem(Item item)
        {
            DragItem(item, item.Amount);
        }

        public static void DragItem(Item item, ushort amount)
        {
            CheckCancellation();

            var pickupPacket = new PickupItemRequest(item.Id, amount);
            ItemsObserver.DraggedItemId = item.Id;
            Program.SendToServer(pickupPacket.RawPacket);
        }

        public static bool MoveItem(Item item, Item targetContainer, TimeSpan? timeout = null,
            TimeSpan? dropDelay = null)
        {
            return MoveItem(item, item.Amount, targetContainer, timeout, dropDelay);
        }

        public static bool MoveItem(Item item, ushort amount, Item targetContainer, TimeSpan? timeout = null,
            TimeSpan? dropDelay = null)
        {
            DragItem(item, amount);
            if (WaitForItemDragged(timeout) != DragResult.Success)
                return false;

            if (dropDelay.HasValue)
                Wait(dropDelay.Value);

            DropItem(item, targetContainer);

            return true;
        }

        public static DragResult WaitForItemDragged(TimeSpan? timeout = null)
        {
            return ItemsObserver.WaitForItemDragged(timeout);
        }

        public static void Log(string message)
        {
            Program.Print(message);
        }

        public static void TriggerGump(uint triggerId)
        {
            GumpObservers.TriggerGump(triggerId);
        }

        public static GumpResponseBuilder GumpResponse()
        {
            return GumpObservers.GumpResponse();
        }

        public static void SelectGumpButton(string buttonLabel, GumpLabelPosition labelPosition)
        {
            GumpObservers.SelectGumpButton(buttonLabel, labelPosition);
        }

        public static void LastGumpInfo()
        {
            var gumpInfo = GumpObservers.LastGumpInfo();
            Log(gumpInfo);
        }

        public static void CloseGump()
        {
            GumpObservers.CloseGump();
        }

        public static void StepToward(Location2D currentLocation, Location2D targetLocation)
        {
            Program.Diagnostic.Debug($"StepToward: {currentLocation} -> {targetLocation}");
            var walkVector = (targetLocation - currentLocation).Normalize();
            if (walkVector != Vector.NullVector)
            {
                Program.Diagnostic.Debug($"StepToward: walkVector = {walkVector}");
                var movementType = Me.CurrentStamina > Me.MaxStamina / 10 ? MovementType.Run : MovementType.Walk;

                var direction = walkVector.ToDirection();
                if (Me.Movement.Direction == direction)
                    WaitToAvoidFastWalk(movementType);

                Walk(direction, movementType);
                WaitWalkAcknowledged();
            }
            else
                Program.Diagnostic.Debug("walkVector is Vector.NullVector");
        }

        public static void StepToward(Item item)
        {
            StepToward((Location2D) item.Location);
        }

        public static void StepToward(Location2D targetLocation)
        {
            StepToward((Location2D) Me.Location, targetLocation);
        }

        public static void WalkTo(Location2D targetLocation)
        {
            while ((Location2D) Me.Location != targetLocation)
            {
                Program.Diagnostic.Debug($"WalkTo: {Me.Location} != {targetLocation}");

                StepToward(targetLocation);
            }
        }

        public static void WalkTo(ushort xloc, ushort yloc)
        {
            WalkTo(new Location2D(xloc, yloc));
        }

        internal static void WalkTo(string parameters)
        {
            var parser = new CommandParameterParser(parameters);
            WalkTo((ushort) parser.ParseInt(), (ushort) parser.ParseInt());
        }

        public static bool Wear(Item item, Layer layer, TimeSpan? timeout = null)
        {
            DragItem(item, 1);
            if (WaitForItemDragged(timeout) != DragResult.Success)
                return false;

            var request = new WearItemRequest(item.Id, layer, Me.PlayerId);
            Program.SendToServer(request.RawPacket);

            return true;
        }

        public static void CastSpell(Spell spell)
        {
            var request = new SkillRequest(spell);

            Program.SendToServer(request.RawPacket);
        }

        public static void UseSkill(Skill skill)
        {
            var request = new SkillRequest(skill);

            Program.SendToServer(request.RawPacket);
        }

        public static void Ignore(Item item)
        {
            ItemsObserver.Ignore(item);
        }
    }
}