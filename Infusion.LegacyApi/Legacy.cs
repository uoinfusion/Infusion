using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Infusion.Commands;
using Infusion.Gumps;
using Infusion.Logging;
using Infusion.Packets;

namespace Infusion.LegacyApi
{
    public static class Legacy
    {
        private static ItemsObservers itemsObserver;
        private static JournalObservers journalObservers;
        private static BlockedPacketsFilters blockedPacketsFilters;
        private static GumpObservers gumpObservers;

        private static readonly ThreadLocal<CancellationToken?> cancellationToken =
            new ThreadLocal<CancellationToken?>(() => null);

        private static Targeting targeting;

        private static JournalSource journalSource;
        private static PlayerObservers playerObservers;

        private static ILogger logger;

        public static LegacyEvents Events { get; private set; }

        public static Configuration Configuration { get; private set; } = new Configuration();

        public static Gump CurrentGump => gumpObservers.CurrentGump;

        public static CancellationToken? CancellationToken
        {
            get => cancellationToken.Value;
            set => cancellationToken.Value = value;
        }

        public static CommandHandler CommandHandler { get; private set; }

        public static UltimaMap Map { get; } = new UltimaMap();

        public static ItemCollection Items { get; set; }

        public static Player Me { get; set; }

        public static GameJournal Journal { get; set; }

        internal static HashSet<uint> IgnoredItems { get; } = new HashSet<uint>();

        public static UltimaServer Server { get; private set; }
        public static UltimaClient Client { get; private set; }

        public static void OpenContainer(Item container, TimeSpan? timeout = null)
        {
            OpenContainer(container.Id);
        }

        public static void OpenContainer(uint containerId, TimeSpan? timeout = null)
        {
            Use(containerId);

            itemsObserver.WaitForContainerOpened(timeout);
        }

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
            logger.Critical(message);
        }

        private static void RegisterDefaultCommands()
        {
            CommandHandler.RegisterCommand(new Command("walkto", parameters => WalkTo(parameters),
                "Walks to the specified location.", "Example: ,walkto 1234 432"));
            CommandHandler.RegisterCommand(new Command("info", InfoCommand,
                "Shows information about selected item or tile."));
            CommandHandler.RegisterCommand(new Command("lastgumpinfo", LastGumpInfo,
                "Shows information about the last gump dispalyed in Ultima Online client."));
            CommandHandler.RegisterCommand(new Command("opendoor", OpenDoor,
                "Opens neares closed doors. This is analogue to UO client's 'opendoor' macro."));
            CommandHandler.RegisterCommand(new Command("terminate", Terminate,
                "Terminates all running commands and scripts.", executionMode: CommandExecutionMode.Direct));
        }

        public static GameJournal CreateJournal()
        {
            return new GameJournal(journalSource);
        }

        public static void Say(string message)
        {
            journalSource.NotifyLastAction();

            Server.Say(message);
        }

        public static Gump WaitForGump(TimeSpan? timeout = null)
        {
            return gumpObservers.WaitForGump(timeout);
        }

        public static void Initialize(Configuration configuration, CommandHandler commandHandler,
            UltimaServer ultimaServer, UltimaClient ultimaClient, ILogger logger)
        {
            Me = new Player(() => Items.OnLayer(Layer.Mount).First() != null, Server);
            gumpObservers = new GumpObservers(ultimaServer, ultimaClient);
            Items = new ItemCollection(Me);
            itemsObserver = new ItemsObservers(Items, ultimaServer);
            Me.LocationChanged += itemsObserver.OnPlayerPositionChanged;
            journalSource = new JournalSource();
            Journal = new GameJournal(journalSource);
            journalObservers = new JournalObservers(journalSource, ultimaServer);
            playerObservers = new PlayerObservers(Me, ultimaClient, ultimaServer, logger);
            playerObservers.WalkRequestDequeued += Me.OnWalkRequestDequeued;
            targeting = new Targeting(ultimaServer, ultimaClient);

            blockedPacketsFilters = new BlockedPacketsFilters(ultimaServer);

            Events = new LegacyEvents(itemsObserver);

            Legacy.logger = logger;
            Server = ultimaServer;
            Client = ultimaClient;

            CommandHandler = commandHandler;
            RegisterDefaultCommands();
            CommandHandler.CancellationTokenCreated += (sender, token) => CancellationToken = token;

            Configuration = configuration;
        }

        public static void Use(uint objectId)
        {
            CheckCancellation();

            journalSource.NotifyLastAction();
            Server.DoubleClick(objectId);
        }

        internal static void CheckCancellation()
        {
            cancellationToken.Value?.ThrowIfCancellationRequested();
        }

        public static void RequestClientStatus(Item item)
        {
            Server.RequestClientStatus(item.Id);
        }

        public static void Use(Item item)
        {
            Use(item.Id);
        }

        public static void Click(Item item)
        {
            Server.Click(item.Id);
        }

        public static bool Use(ItemSpec spec)
        {
            CheckCancellation();

            var item = Items.Matching(spec).OnLayer(Layer.OneHandedWeapon).First()
                       ?? Items.Matching(spec).OnLayer(Layer.TwoHandedWeapon).First()
                       ?? Items.Matching(spec).InContainer(Me.BackPack).First()
                       ?? Items.Matching(spec).OnLayer(Layer.Backpack).First();

            if (item != null)
                Use(item);
            else
                return false;

            return true;
        }

        public static bool UseType(ModelId type)
        {
            CheckCancellation();

            var item = Items.OfType(type).OnLayer(Layer.OneHandedWeapon).First()
                       ?? Items.OfType(type).OnLayer(Layer.TwoHandedWeapon).First()
                       ?? Items.OfType(type).InContainer(Me.BackPack).First()
                       ?? Items.OfType(type).OnLayer(Layer.Backpack).First();
            if (item != null)
            {
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
            Server.RequestWarMode(WarMode.Fighting);
        }

        public static void WarModeOff()
        {
            Server.RequestWarMode(WarMode.Normal);
        }

        public static AttackResult Attack(Item target, TimeSpan? timeout = null)
        {
            return playerObservers.Attack(target.Id, timeout);
        }

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

            Server.DropItem(item.Id, targetContainer.Id);
        }

        public static void DragItem(Item item)
        {
            DragItem(item, item.Amount);
        }

        public static void DragItem(Item item, ushort amount)
        {
            CheckCancellation();
            itemsObserver.DraggedItemId = item.Id;

            Server.DragItem(item.Id, amount);
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
            logger.Info(message);
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
            var walkVector = (targetLocation - currentLocation).Normalize();
            if (walkVector != Vector.NullVector)
            {
                var movementType = Me.CurrentStamina > Me.MaxStamina / 10 ? MovementType.Run : MovementType.Walk;

                var direction = walkVector.ToDirection();
                if (Me.Movement.Direction == direction)
                    WaitToAvoidFastWalk(movementType);

                Walk(direction, movementType);
                WaitWalkAcknowledged();
            }
        }

        public static void StepToward(Item item)
        {
            StepToward(item.Location);
        }

        public static void StepToward(Location2D targetLocation)
        {
            StepToward(Me.Location, targetLocation);
        }

        public static void WalkTo(Location2D targetLocation)
        {
            while (Me.Location != targetLocation)
            {
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

            Server.Wear(item.Id, layer, Me.PlayerId);

            return true;
        }

        public static void CastSpell(Spell spell)
        {
            journalSource.NotifyLastAction();

            Server.CastSpell(spell);
        }

        public static void UseSkill(Skill skill)
        {
            journalSource.NotifyLastAction();

            Server.UseSkill(skill);
        }

        public static void OpenDoor()
        {
            Server.OpenDoor();
        }

        public static void Ignore(Item item)
        {
            IgnoredItems.Add(item.Id);
        }

        public static void ClientPrint(string message, string name, uint itemId, ModelId itemModel, SpeechType type,
            Color color)
        {
            Client.SendSpeech(message, name, itemId, itemModel, type, color);
        }

        public static void ClientPrintAndLog(string message)
        {
            ClientPrint(message);
            logger.Info(message);
        }

        public static void ClientPrint(string message)
        {
            ClientPrint(message, "System", 0, 0, SpeechType.Normal, (Color) 0x03B2);
        }

        public static void ClientPrint(string message, string name, Player onBehalfPlayer)
        {
            ClientPrint(message, name, onBehalfPlayer.PlayerId, onBehalfPlayer.BodyType, SpeechType.Speech,
                (Color) 0x0026);
        }

        public static void ClientPrint(string message, string name, Item onBehalfItem)
        {
            ClientPrint(message, name, onBehalfItem.Id, onBehalfItem.Type, SpeechType.Speech, (Color) 0x0026);
        }

        public static void CloseContainer(Item container)
        {
            Client.CloseContainer(container.Id);
        }
    }
}