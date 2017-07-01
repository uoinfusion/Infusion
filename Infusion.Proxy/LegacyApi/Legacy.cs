using System;
using System.Collections.Generic;
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
        private static readonly ItemsObservers itemsObserver;
        private static readonly JournalObservers journalObservers;
        private static readonly BlockedPacketsFilters blockedPacketsFilters;
        private static readonly CommandHandlerObservers legacyCommandHandler;
        private static readonly GumpObservers gumpObservers;

        private static readonly ThreadLocal<CancellationToken?> cancellationToken =
            new ThreadLocal<CancellationToken?>(() => null);

        private static readonly Targeting targeting;

        private static readonly JournalSource journalSource;
        private static readonly PlayerObservers playerObservers;

        static Legacy()
        {
            gumpObservers = new GumpObservers(Program.ServerPacketHandler, Program.ClientPacketHandler);
            Items = new ItemCollection(Me);
            itemsObserver = new ItemsObservers(Items, Program.ServerPacketHandler);
            Me.LocationChanged += itemsObserver.OnPlayerPositionChanged;
            journalSource = new JournalSource();
            Journal = new GameJournal(journalSource);
            journalObservers = new JournalObservers(journalSource, Program.ServerPacketHandler);
            playerObservers = new PlayerObservers(Me, Program.ClientPacketHandler, Program.ServerPacketHandler);
            playerObservers.WalkRequestDequeued += Me.OnWalkRequestDequeued;
            targeting = new Targeting(Program.ServerPacketHandler, Program.ClientPacketHandler);

            RegisterDefaultCommands();

            legacyCommandHandler = new CommandHandlerObservers(Program.ClientPacketHandler, CommandHandler);
            blockedPacketsFilters = new BlockedPacketsFilters(Program.ServerPacketHandler);
        }

        public static Configuration Configuration { get; private set; } = new Configuration();

        public static Gump CurrentGump => gumpObservers.CurrentGump;

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
            get => itemsObserver.HitPointNotificationEnabled;

            set => itemsObserver.HitPointNotificationEnabled = value;
        }

        public static void OpenContainer(Item container, TimeSpan? timeout = null) => OpenContainer(container.Id);

        public static void OpenContainer(uint containerId, TimeSpan? timeout = null)
        {
            Use(containerId);

            itemsObserver.WaitForContainerOpened(timeout);
        }

        public static void CloseContainer(Item container) => CloseContainer(container.Id);

        public static void CloseContainer(uint containerId)
        {
            Program.SendToClient(new CloseContainerPacket(containerId).RawPacket);
        }

        public static GameJournal Journal { get; }

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

        private static void RegisterDefaultCommands()
        {
            CommandHandler.RegisterCommand(new Command("terminate", Terminate,
                "Terminates all running commands and scripts.", executionMode: CommandExecutionMode.Direct));
            CommandHandler.RegisterCommand(new Command("dump", Program.DumpPacketLog,
                "Dumps packet log - log of network communication between game client and server. Network communication logs are very useful for diagnosing issues like crashes.", executionMode: CommandExecutionMode.Direct));
            CommandHandler.RegisterCommand(new Command("walkto", parameters => WalkTo(parameters),
                "Walks to the specified location.", "Example: ,walkto 1234 432"));
            CommandHandler.RegisterCommand(new Command("info", InfoCommand,
                "Shows information about selected item or tile."));
            CommandHandler.RegisterCommand(new Command("help", HelpCommand, "Shows command help."));
            CommandHandler.RegisterCommand(new Command("lastgumpinfo", LastGumpInfo,
                "Shows information about the last gump dispalyed in Ultima Online client."));
            CommandHandler.RegisterCommand(new Command("list", ListRunningCommands,
                "Lists running commands"));
            CommandHandler.RegisterCommand(new Command("opendoor", OpenDoor,
                "Opens neares closed doors. This is analogue to UO client's 'opendoor' macro."));
        }

        public static GameJournal CreateJournal() => new GameJournal(journalSource);

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
            journalSource.NotifyLastAction();

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

        public static void ClientPrint(string message, string name, Player onBehalfPlayer)
        {
            ClientPrint(message, name, onBehalfPlayer.PlayerId, onBehalfPlayer.BodyType, SpeechType.Speech, (Color)0x0026);
        }

        public static void ClientPrint(string message, string name, Item onBehalfItem)
        {
            ClientPrint(message, name, onBehalfItem.Id, onBehalfItem.Type, SpeechType.Speech, (Color) 0x0026);
        }

        public static Gump WaitForGump(TimeSpan? timeout = null)
        {
            return gumpObservers.WaitForGump(timeout);
        }

        public static void Initialize(Configuration configuration)
        {
            Configuration = configuration;
        }

        public static void Use(uint objectId)
        {
            CheckCancellation();

            journalSource.NotifyLastAction();
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
            Log($"Using item: {item}");
            Use(item.Id);
        }

        public static bool Use(ItemSpec spec)
        {
            CheckCancellation();

            var item = Items.Matching(spec).InContainer(Me.BackPack).First()
                       ?? Items.Matching(spec).OnLayer(Layer.OneHandedWeapon).First()
                       ?? Items.Matching(spec).OnLayer(Layer.TwoHandedWeapon).First()
                       ?? Items.Matching(spec).OnLayer(Layer.Backpack).First();

            if (item != null)
            {
                Use(item);
            }
            else
                return false;

            return true;
        }

        public static bool UseType(ModelId type)
        {
            CheckCancellation();

            var item = Items.OfType(type).InContainer(Me.BackPack).First()
                       ?? Items.OfType(type).OnLayer(Layer.OneHandedWeapon).First()
                       ?? Items.OfType(type).OnLayer(Layer.TwoHandedWeapon).First()
                       ?? Items.OfType(type).OnLayer(Layer.Backpack).First();
            if (item != null)
            {
                Log($"Using item: {item}");
                Use(item);
            }
            else
                return false;

            return true;
        }

        public static bool UseType(params ModelId[] types)
        {
            CheckCancellation();

            var item = Items.OfType(types).InContainer(Me.BackPack).First()
                       ?? Items.OfType(types).OnLayer(Layer.OneHandedWeapon).First()
                       ?? Items.OfType(types).OnLayer(Layer.TwoHandedWeapon).First()
                       ?? Items.OfType(types).OnLayer(Layer.Backpack).First();

            if (item != null)
            {
                Log($"Using item: {item}");
                Use(item);
                return true;
            }

            var typesString = types.Select(u => u.ToString()).Aggregate(string.Empty, (l, r) => l + ", " + r);
            Log($"Item of any type {typesString} not found.");

            return false;
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

        public static AttackResult Attack(Item target, TimeSpan? timeout = null) =>
            playerObservers.Attack(target.Id, timeout);

        public static void TargetTile(string tileInfo)
        {
            CheckCancellation();

            journalSource.NotifyLastAction();
            targeting.TargetTile(tileInfo);
        }

        public static void Target(Item item)
        {
            CheckCancellation();

            journalSource.NotifyLastAction();
            targeting.Target(item);
        }

        public static void Target(Player player)
        {
            CheckCancellation();

            journalSource.NotifyLastAction();
            targeting.Target(player);
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
            return targeting.Info();
        }

        private static void InfoCommand()
        {
            var info = Info();
            ClientPrintAndLog(!string.IsNullOrEmpty(info) ? info : "Targeting cancelled.");
        }

        public static ModelId TypeInfo()
        {
            return targeting.TypeInfo();
        }

        public static Item ItemInfo()
        {
            var itemId = targeting.ItemIdInfo();

            Item item;
            if (!Items.TryGet(itemId, out item))
                return null;

            return item;
        }

        public static void WaitForTarget()
        {
            CheckCancellation();

            targeting.WaitForTarget();
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
            itemsObserver.DraggedItemId = item.Id;
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
            return itemsObserver.WaitForItemDragged(timeout);
        }

        public static void Log(string message)
        {
            Program.Print(message);
        }

        public static void TriggerGump(uint triggerId)
        {
            gumpObservers.TriggerGump(triggerId);
        }

        public static GumpResponseBuilder GumpResponse()
        {
            return gumpObservers.GumpResponse();
        }

        public static void SelectGumpButton(string buttonLabel, GumpLabelPosition labelPosition)
        {
            gumpObservers.SelectGumpButton(buttonLabel, labelPosition);
        }

        public static void LastGumpInfo()
        {
            var gumpInfo = gumpObservers.LastGumpInfo();
            Log(gumpInfo);
        }

        public static void CloseGump()
        {
            gumpObservers.CloseGump();
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
            journalSource.NotifyLastAction();

            var request = new SkillRequest(spell);
            Program.SendToServer(request.RawPacket);
        }

        public static void UseSkill(Skill skill)
        {
            journalSource.NotifyLastAction();

            var request = new SkillRequest(skill);
            Program.SendToServer(request.RawPacket);
        }

        public static void OpenDoor()
        {
            var request = new SkillRequest(0x58, null);
            Program.SendToServer(request.RawPacket);
        }

        internal static HashSet<uint> IgnoredItems { get; } = new HashSet<uint>();

        public static void Ignore(Item item)
        {
            IgnoredItems.Add(item.Id);
        }
    }
}