using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Infusion.Commands;
using Infusion.Gumps;
using Infusion.Logging;

namespace Infusion.LegacyApi
{
    public class Legacy
    {
        private readonly BlockedClientPacketsFilters blockedPacketsFilters;

        private readonly ThreadLocal<CancellationToken?> cancellationToken =
            new ThreadLocal<CancellationToken?>(() => null);

        private readonly GumpObservers gumpObservers;
        private readonly ItemsObservers itemsObserver;
        private readonly JournalObservers journalObservers;

        private readonly JournalSource journalSource;
        private readonly LightObserver lightObserver;

        private readonly ILogger logger;
        private readonly PlayerObservers playerObservers;

        private readonly Targeting targeting;
        private readonly WeatherObserver weatherObserver;

        internal Legacy(Configuration configuration, CommandHandler commandHandler,
            UltimaServer ultimaServer, UltimaClient ultimaClient, ILogger logger)
        {
            Me = new Player(() => GameObjects.OfType<Item>().OnLayer(Layer.Mount).FirstOrDefault() != null,
                ultimaServer, this);
            gumpObservers = new GumpObservers(ultimaServer, ultimaClient, this);
            GameObjects = new GameObjectCollection(Me);
            Items = new ItemCollection(GameObjects);
            Mobiles = new MobileCollection(GameObjects);
            itemsObserver = new ItemsObservers(GameObjects, ultimaServer, ultimaClient, this);
            Me.LocationChanged += itemsObserver.OnPlayerPositionChanged;
            journalSource = new JournalSource();
            Journal = new GameJournal(journalSource, this);
            journalObservers = new JournalObservers(journalSource, ultimaServer);
            playerObservers = new PlayerObservers(Me, ultimaClient, ultimaServer, logger, this);
            playerObservers.WalkRequestDequeued += Me.OnWalkRequestDequeued;
            targeting = new Targeting(ultimaServer, ultimaClient, this);

            blockedPacketsFilters = new BlockedClientPacketsFilters(ultimaClient);
            lightObserver = new LightObserver(ultimaServer, ultimaClient, configuration, Me);
            weatherObserver = new WeatherObserver(ultimaServer, ultimaClient, configuration);

            Events = new LegacyEvents(itemsObserver, journalSource);

            this.logger = logger;
            Server = ultimaServer;
            Client = ultimaClient;

            CommandHandler = commandHandler;
            RegisterDefaultCommands();
            CommandHandler.CancellationTokenCreated += (sender, token) => CancellationToken = token;

            Configuration = configuration;
        }

        public LegacyEvents Events { get; }

        public Configuration Configuration { get; }

        public Gump CurrentGump => gumpObservers.CurrentGump;

        public CancellationToken? CancellationToken
        {
            get => cancellationToken.Value;
            set => cancellationToken.Value = value;
        }

        public CommandHandler CommandHandler { get; }

        public UltimaMap Map { get; } = new UltimaMap();

        internal GameObjectCollection GameObjects { get; }
        public ItemCollection Items { get; }
        public MobileCollection Mobiles { get; }

        public Player Me { get; }

        public GameJournal Journal { get; }

        public ISet<ObjectId> IgnoredItems { get; } = new HashSet<ObjectId>();

        internal UltimaServer Server { get; }
        internal UltimaClient Client { get; }

        public void OpenContainer(Item container, TimeSpan? timeout = null)
        {
            OpenContainer(container.Id);
        }

        public void OpenContainer(ObjectId containerId, TimeSpan? timeout = null)
        {
            Use(containerId);

            itemsObserver.WaitForContainerOpened(timeout);
        }

        public Command RegisterCommand(string name, Action commandAction) => CommandHandler.RegisterCommand(name,
            commandAction);

        public Command RegisterCommand(string name, Action<string> commandAction) => CommandHandler.RegisterCommand(
            name,
            commandAction);

        public void Alert(string message)
        {
            logger.Critical(message);
        }

        private void RegisterDefaultCommands()
        {
            CommandHandler.RegisterCommand(new Command("walkto", parameters => WalkTo(parameters),
                "Walks to the specified location.", "Example: ,walkto 1234 432"));
            CommandHandler.RegisterCommand(new Command("info", InfoCommand,
                "Shows information about selected item or tile."));
            CommandHandler.RegisterCommand(new Command("lastgumpinfo", LastGumpInfo,
                "Shows information about the last gump dispalyed in Ultima Online client."));
            CommandHandler.RegisterCommand(new Command("opendoor", OpenDoor,
                "Opens neares closed doors. This is analogue to UO client's 'opendoor' macro."));
            CommandHandler.RegisterCommand(new Command("warmode-on", WarModeOn,
                "War mode on."));
            CommandHandler.RegisterCommand(new Command("warmode-off", WarModeOff,
                "War mode off."));
            CommandHandler.RegisterCommand(new Command("terminate", Terminate,
                "Terminates all running commands and scripts.", executionMode: CommandExecutionMode.Direct));
            CommandHandler.RegisterCommand(new Command("filter-light", ToggleLightFiltering));
            CommandHandler.RegisterCommand(new Command("filter-weather", ToggleWeatherFiltering));
        }

        public GameJournal CreateJournal()
        {
            return new GameJournal(journalSource, this);
        }

        public void Say(string message)
        {
            journalSource.NotifyLastAction();

            Server.Say(message);
        }

        public Gump WaitForGump(TimeSpan? timeout = null)
        {
            return gumpObservers.WaitForGump(timeout);
        }

        public void Use(ObjectId objectId)
        {
            CheckCancellation();

            journalSource.NotifyLastAction();
            Server.DoubleClick(objectId);
        }

        internal void CheckCancellation()
        {
            cancellationToken.Value?.ThrowIfCancellationRequested();
        }

        public void RequestStatus(Mobile item) => Server.RequestStatus(item.Id);

        public void Use(GameObject item) => Use(item.Id);

        public void Click(GameObject obj) => Server.Click(obj.Id);

        public void Click(ObjectId id) => Server.Click(id);

        public bool TryUse(ItemSpec spec)
        {
            CheckCancellation();

            var item = Items.Matching(spec).OnLayer(Layer.OneHandedWeapon)
                           .FirstOrDefault(i => i.ContainerId.HasValue && i.ContainerId == Me.PlayerId)
                       ?? Items.Matching(spec).OnLayer(Layer.TwoHandedWeapon)
                           .FirstOrDefault(i => i.ContainerId.HasValue && i.ContainerId == Me.PlayerId)
                       ?? Items.Matching(spec).InContainer(Me.BackPack).FirstOrDefault()
                       ?? Items.Matching(spec).OnLayer(Layer.Backpack)
                           .FirstOrDefault(i => i.ContainerId.HasValue && i.ContainerId == Me.PlayerId);

            if (item != null)
            {
                Use(item);
                return true;
            }

            return false;
        }

        public void Use(ItemSpec spec)
        {
            if (!TryUse(spec))
                throw new LegacyException($"Cannot find item {spec}.");
        }

        public bool TryUse(ModelId type)
        {
            CheckCancellation();

            var item = Items.OfType(type).OnLayer(Layer.OneHandedWeapon)
                           .FirstOrDefault(i => i.ContainerId.HasValue && i.ContainerId == Me.PlayerId)
                       ?? Items.OfType(type).OnLayer(Layer.TwoHandedWeapon)
                           .FirstOrDefault(i => i.ContainerId.HasValue && i.ContainerId == Me.PlayerId)
                       ?? Items.OfType(type).InContainer(Me.BackPack).FirstOrDefault()
                       ?? Items.OfType(type).OnLayer(Layer.Backpack)
                           .FirstOrDefault(i => i.ContainerId.HasValue && i.ContainerId == Me.PlayerId);
            if (item != null)
            {
                Use(item);
                return true;
            }

            return false;
        }

        public void Use(ModelId type)
        {
            if (!TryUse(type))
                throw new LegacyException($"Cannot find of type {type}.");
        }

        public bool TryUse(params ModelId[] types)
        {
            CheckCancellation();

            var item = Items.OfType(types).InContainer(Me.BackPack)
                           .FirstOrDefault(i => i.ContainerId.HasValue && i.ContainerId == Me.PlayerId)
                       ?? Items.OfType(types).OnLayer(Layer.OneHandedWeapon)
                           .FirstOrDefault(i => i.ContainerId.HasValue && i.ContainerId == Me.PlayerId)
                       ?? Items.OfType(types).OnLayer(Layer.TwoHandedWeapon).FirstOrDefault()
                       ?? Items.OfType(types).OnLayer(Layer.Backpack)
                           .FirstOrDefault(i => i.ContainerId.HasValue && i.ContainerId == Me.PlayerId);

            if (item != null)
            {
                Use(item);
                return true;
            }

            return false;
        }

        public void Use(params ModelId[] types)
        {
            if (!TryUse(types))
            {
                var typesString = types.Select(u => u.ToString()).Aggregate(string.Empty, (l, r) => l + ", " + r);
                throw new LegacyException($"Item of any type {typesString} not found.");
            }
        }

        public void Wait(int milliseconds)
        {
            while (milliseconds > 0)
            {
                CheckCancellation();
                Thread.Sleep(50);
                milliseconds -= 50;
            }
        }

        public void Wait(TimeSpan span)
        {
            Wait((int) span.TotalMilliseconds);
        }

        public void WaitToAvoidFastWalk(MovementType movementType)
        {
            Me.WaitToAvoidFastWalk(movementType);
        }

        public void WaitWalkAcknowledged()
        {
            CheckCancellation();
            Me.WaitWalkAcknowledged();
        }

        public void Walk(Direction direction, MovementType movementType = MovementType.Walk)
        {
            CheckCancellation();

            Me.Walk(direction, movementType);
        }

        public void WarModeOn()
        {
            Server.RequestWarMode(WarMode.Fighting);
        }

        public void WarModeOff()
        {
            Server.RequestWarMode(WarMode.Normal);
        }

        public AttackResult TryAttack(Mobile target, TimeSpan? timeout = null)
        {
            return playerObservers.Attack(target.Id, timeout);
        }

        public void TargetTile(string tileInfo)
        {
            CheckCancellation();

            journalSource.NotifyLastAction();
            targeting.TargetTile(tileInfo);
        }

        public void Target(GameObject item)
        {
            CheckCancellation();

            journalSource.NotifyLastAction();
            targeting.Target(item);
        }

        public void Target(Player player)
        {
            CheckCancellation();

            journalSource.NotifyLastAction();
            targeting.Target(player);
        }

        public void Terminate(string parameters)
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

        public string Info()
        {
            return targeting.Info();
        }

        private void InfoCommand()
        {
            var info = Info();
            ClientPrint(!string.IsNullOrEmpty(info) ? info : "Targeting cancelled.");
        }

        public Item AskForItem()
        {
            var itemId = targeting.ItemIdInfo();

            if (!GameObjects.TryGet(itemId, out var obj))
                return null;

            return obj as Item;
        }

        public Mobile AskForMobile()
        {
            var itemId = targeting.ItemIdInfo();

            if (!GameObjects.TryGet(itemId, out var obj))
                return null;

            return obj as Mobile;
        }

        public void WaitForTarget()
        {
            CheckCancellation();

            targeting.WaitForTarget();
        }

        public void DropItem(Item item, Item targetContainer)
        {
            DropItem(item.Id, targetContainer.Id);
        }

        internal void DropItem(Item item, ObjectId targetContainerId)
        {
            DropItem(item.Id, targetContainerId);
        }

        internal void DropItem(ObjectId itemId, ObjectId targetContainerId)
        {
            CheckCancellation();

            Server.DropItem(itemId, targetContainerId);
        }

        public void DragItem(Item item)
        {
            DragItem(item, item.Amount);
        }

        public void DragItem(Item item, ushort amount)
        {
            CheckCancellation();
            itemsObserver.DraggedItemId = item.Id;

            Server.DragItem(item.Id, amount);
        }

        public bool TryMoveItem(Item item, Item targetContainer, TimeSpan? timeout = null,
            TimeSpan? dropDelay = null) => TryMoveItem(item, item.Amount, targetContainer.Id, timeout, dropDelay);

        public bool TryMoveItem(Item item, ObjectId targetContainerId, TimeSpan? timeout = null,
            TimeSpan? dropDelay = null) => TryMoveItem(item, item.Amount, targetContainerId, timeout, dropDelay);

        public bool TryMoveItem(Item item, ushort amount, Item targetContainer, TimeSpan? timeout = null,
            TimeSpan? dropDelay = null) => TryMoveItem(item, item.Amount, targetContainer.Id, timeout, dropDelay);

        public bool TryMoveItem(Item item, ushort amount, ObjectId targetContainerId, TimeSpan? timeout = null,
            TimeSpan? dropDelay = null)
        {
            DragItem(item, amount);
            if (WaitForItemDragged(timeout) != DragResult.Success)
                return false;

            if (dropDelay.HasValue)
                Wait(dropDelay.Value);

            DropItem(item, targetContainerId);

            return true;
        }

        public DragResult WaitForItemDragged(TimeSpan? timeout = null)
        {
            return itemsObserver.WaitForItemDragged(timeout);
        }

        public void Log(string message)
        {
            logger.Info(message);
        }

        public void TriggerGump(GumpControlId triggerId)
        {
            gumpObservers.TriggerGump(triggerId);
        }

        public GumpResponseBuilder GumpResponse()
        {
            return gumpObservers.GumpResponse();
        }

        public void SelectGumpButton(string buttonLabel, GumpLabelPosition labelPosition)
        {
            gumpObservers.SelectGumpButton(buttonLabel, labelPosition);
        }

        public void LastGumpInfo()
        {
            var gumpInfo = gumpObservers.LastGumpInfo();
            Log(gumpInfo);
        }

        public void CloseGump()
        {
            gumpObservers.CloseGump();
        }

        public void StepToward(Location2D currentLocation, Location2D targetLocation)
        {
            var walkVector = (targetLocation - currentLocation).Normalize();
            if (walkVector != Vector.NullVector)
            {
                var movementType = Me.CurrentStamina > Me.MaxStamina / 10 ? MovementType.Run : MovementType.Walk;

                var direction = walkVector.ToDirection();
                if (Me.Direction == direction)
                    WaitToAvoidFastWalk(movementType);

                Walk(direction, movementType);
                WaitWalkAcknowledged();
            }
        }

        public void StepToward(GameObject gameObject)
        {
            StepToward(gameObject.Location);
        }

        public void StepToward(Location2D targetLocation)
        {
            StepToward(Me.Location, targetLocation);
        }

        public void WalkTo(Location2D targetLocation)
        {
            while (Me.Location != targetLocation)
            {
                StepToward(targetLocation);
            }
        }

        public void WalkTo(ushort xloc, ushort yloc)
        {
            WalkTo(new Location2D(xloc, yloc));
        }

        internal void WalkTo(string parameters)
        {
            var parser = new CommandParameterParser(parameters);
            WalkTo((ushort) parser.ParseInt(), (ushort) parser.ParseInt());
        }

        public void Wear(Item item, Layer layer, TimeSpan? timeout = null)
        {
            if (!TryWear(item, layer, timeout))
                throw new LegacyException($"Cannot pickup {item}");
        }

        public bool TryWear(Item item, Layer layer, TimeSpan? timeout = null)
        {
            DragItem(item, 1);
            if (WaitForItemDragged(timeout) != DragResult.Success)
                return false;

            Server.Wear(item.Id, layer, Me.PlayerId);

            return true;
        }

        public void CastSpell(Spell spell)
        {
            journalSource.NotifyLastAction();

            Server.CastSpell(spell);
        }

        public void UseSkill(Skill skill)
        {
            journalSource.NotifyLastAction();

            Server.UseSkill(skill);
        }

        public void OpenDoor()
        {
            Server.OpenDoor();
        }

        public void Ignore(Item item)
        {
            IgnoredItems.Add(item.Id);
        }

        public void ClientPrint(string message, string name, ObjectId itemId, ModelId itemModel, SpeechType type,
            Color color, bool log = true)
        {
            Client.SendSpeech(message, name, itemId, itemModel, type, color);
            if (log)
                Log(message);
        }

        public void ClientPrint(string message, bool log = true)
        {
            ClientPrint(message, "System", new ObjectId(0), 0, SpeechType.Normal, (Color) 0x03B2, log);
        }

        public void ClientPrint(string message, string name, Player onBehalfPlayer, bool log = true)
        {
            ClientPrint(message, name, onBehalfPlayer.PlayerId, onBehalfPlayer.BodyType, SpeechType.Speech,
                (Color) 0x0026, log);
        }

        public void ClientPrint(string message, string name, Item onBehalfItem, bool log = true)
        {
            ClientPrint(message, name, onBehalfItem.Id, onBehalfItem.Type, SpeechType.Speech, (Color) 0x0026, log);
        }

        public void CloseContainer(Item container)
        {
            Client.CloseContainer(container.Id);
        }

        public void ToggleLightFiltering()
        {
            lightObserver.ToggleLightFiltering();
            ClientPrint(Configuration.FilterLightEnabled ? "Light filtering turned on" : "Light filtering turned off");
        }

        public void ToggleWeatherFiltering()
        {
            weatherObserver.ToggleWeatherFiltering();
            ClientPrint(Configuration.FilterWeatherEnabled ? "Weather filtering turned on" : "Weather filtering turned off");
        }
    }
}