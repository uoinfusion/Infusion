using System;
using System.Linq;
using System.Threading;
using Infusion.Commands;
using Infusion.Gumps;
using Infusion.LegacyApi.Events;
using Infusion.LegacyApi.Filters;
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
        private readonly EventJournal legacyEventJournal;

        private readonly JournalSource journalSource;
        private readonly LightObserver lightObserver;

        private readonly ILogger logger;
        private readonly PlayerObservers playerObservers;
        private readonly QuestArrowObserver questArrowObserver;
        private readonly SoundObserver soundObserver;

        private readonly Targeting targeting;
        private readonly WeatherObserver weatherObserver;
        private readonly Cancellation cancellation;
        private readonly EventJournalSource eventJournalSource;
        private readonly DialogBoxObservers dialogBoxObervers;

        internal Legacy(Configuration configuration, CommandHandler commandHandler,
            UltimaServer ultimaServer, UltimaClient ultimaClient, ILogger logger)
        {
            cancellation = new Cancellation(() => CancellationToken);
            eventJournalSource = new EventJournalSource();
            Me = new Player(() => GameObjects.OfType<Item>().OnLayer(Layer.Mount).FirstOrDefault() != null,
                ultimaServer, this, eventJournalSource);
            gumpObservers = new GumpObservers(ultimaServer, ultimaClient, eventJournalSource, cancellation);
            GameObjects = new GameObjectCollection(Me);
            Items = new ItemCollection(GameObjects);
            Mobiles = new MobileCollection(GameObjects);
            itemsObserver = new ItemsObservers(GameObjects, ultimaServer, ultimaClient, this, eventJournalSource);
            Me.LocationChanged += itemsObserver.OnPlayerPositionChanged;
            journalSource = new JournalSource();
            journalSource.NewMessageReceived +=
                (sender, entry) => eventJournalSource.Publish(new SpeechReceivedEvent(entry));
            Journal = new SpeechJournal(journalSource, cancellation, () => DefaultTimeout);
            journalObservers = new JournalObservers(journalSource, ultimaServer);
            targeting = new Targeting(ultimaServer, ultimaClient, cancellation);

            blockedPacketsFilters = new BlockedClientPacketsFilters(ultimaClient);
            lightObserver = new LightObserver(ultimaServer, ultimaClient, Me, this);
            weatherObserver = new WeatherObserver(ultimaServer, ultimaClient, this);
            soundObserver = new SoundObserver(ultimaServer, eventJournalSource);
            questArrowObserver = new QuestArrowObserver(ultimaServer, eventJournalSource);
            var speechRequestObserver = new SpeechRequestObserver(ultimaClient, commandHandler, eventJournalSource);
            var staminaFilter = new StaminaFilter(ultimaServer, ultimaClient);
            dialogBoxObervers = new DialogBoxObservers(ultimaServer, eventJournalSource);

            playerObservers = new PlayerObservers(Me, ultimaClient, ultimaServer, logger, this, GameObjects, eventJournalSource);
            playerObservers.WalkRequestDequeued += Me.OnWalkRequestDequeued;

            this.logger = logger;
            Server = ultimaServer;
            Client = ultimaClient;

            CommandHandler = commandHandler;
            CommandHandler.CancellationTokenCreated += (sender, token) => CancellationToken = token;

            Configuration = configuration;
            legacyEventJournal = CreateEventJournal();

            ClientFilters = new LegacyFilters(staminaFilter, lightObserver, weatherObserver, soundObserver);
            RegisterDefaultCommands();
        }

        public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(30);

        public Configuration Configuration { get; }

        public LegacyFilters ClientFilters { get; }

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

        public SpeechJournal Journal { get; }

        internal UltimaServer Server { get; }
        public UltimaClient Client { get; }
        public IUltimaClientWindow ClientWindow { get; internal set; } = new NullUltimaClientWindow();

        public Command RegisterCommand(string name, Action commandAction) => CommandHandler.RegisterCommand(name,
            commandAction);

        public Command RegisterCommand(string name, Action<string> commandAction) => CommandHandler.RegisterCommand(
            name,
            commandAction);

        public Command RegisterBackgroundCommand(string name, Action commandAction)
        {
            var command = new Command(name, commandAction, string.Empty, string.Empty, CommandExecutionMode.Background);
            CommandHandler.RegisterCommand(command);

            return command;
        }

        public Command RegisterBackgroundCommand(string name, Action<string> commandAction)
        {
            var command = new Command(name, commandAction, string.Empty, string.Empty, CommandExecutionMode.Background);
            CommandHandler.RegisterCommand(command);

            return command;
        }

        public void Alert(string message)
        {
            logger.Critical(message);
        }

        private void RegisterDefaultCommands()
        {
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
                "Terminates all running commands excluding background commands.", executionMode: CommandExecutionMode.Direct));
            CommandHandler.RegisterCommand(new Command("terminate-all", CommandHandler.TerminateAll,
                "Terminates all running commands.", executionMode: CommandExecutionMode.Direct));
            CommandHandler.RegisterCommand(new Command("filter-light", ClientFilters.Light.Toggle));
            CommandHandler.RegisterCommand(new Command("filter-weather", ClientFilters.Weather.Toggle));
        }

        public SpeechJournal CreateSpeechJournal() => new SpeechJournal(journalSource, cancellation, () => DefaultTimeout);
        public EventJournal CreateEventJournal() => new EventJournal(eventJournalSource, cancellation, () => DefaultTimeout);

        public void Say(string message)
        {
            NotifyAction();

            Server.Say(message);
        }

        public void NotifyAction()
        {
            journalSource.NotifyAction();
            targeting.NotifyLastAction(DateTime.UtcNow);
        }

        public Gump WaitForGump(bool showGump = true, TimeSpan? timeout = null)
        {
            return gumpObservers.WaitForGump(showGump, timeout);
        }

        public void Use(ObjectId objectId)
        {
            CheckCancellation();

            NotifyAction();
            Server.DoubleClick(objectId);
        }

        public void CheckCancellation()
        {
            cancellationToken.Value?.ThrowIfCancellationRequested();
        }

        public void RequestStatus(Mobile item) => Server.RequestStatus(item.Id);
        public void RequestStatus(ObjectId id) => Server.RequestStatus(id);

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
                       ?? Items.Matching(spec).InBackPack().FirstOrDefault()
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
                       ?? Items.OfType(type).InBackPack().FirstOrDefault()
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

            var item = Items.OfType(types).InBackPack()
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

        private void WaitToAvoidFastWalk(MovementType movementType)
        {
            Me.WaitToAvoidFastWalk(movementType);
        }

        private bool WaitWalkAcknowledged()
        {
            CheckCancellation();
            return Me.WaitWalkAcknowledged();
        }

        public bool Walk(Direction direction, MovementType movementType = MovementType.Walk)
        {
            CheckCancellation();

            if (UO.Me.Direction == direction)
                WaitToAvoidFastWalk(MovementType.Run);

            Me.Walk(direction, movementType);
            return WaitWalkAcknowledged();
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

            NotifyAction();
            targeting.TargetTile(tileInfo);
        }

        public void Target(Location2D location)
        {
            CheckCancellation();
            NotifyAction();

            targeting.TargetTile(location.X, location.Y, 0, 0);
        }

        public void Target(Location3D location)
        {
            CheckCancellation();
            NotifyAction();

            targeting.TargetTile(location.X, location.Y, 0, 0);
        }

        public void Target(TargetInfo targetInfo)
        {
            CheckCancellation();

            NotifyAction();
            targeting.Target(targetInfo);
        }

        public void Target(GameObject item)
        {
            CheckCancellation();

            NotifyAction();
            targeting.Target(item);
        }

        public void Target(Player player)
        {
            CheckCancellation();

            NotifyAction();
            targeting.Target(player);
        }

        public void Target(ObjectId id)
        {
            CheckCancellation();
            NotifyAction();

            var gameObject = GameObjects[id];
            if (gameObject == null)
                targeting.Target(id);
            else
                targeting.Target(gameObject);
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

        public TargetInfo? Info() => targeting.Info();

        private void InfoCommand()
        {
            var info = Info();
            if (info.HasValue)
            {
                if (info.Value.Id.HasValue)
                {
                    var lastObject = GameObjects[info.Value.Id.Value];
                    if (lastObject != null)
                    {
                        ClientPrint(lastObject.ToString());
                        return;
                    }
                }

                ClientPrint(info.Value.ToString());
            }
            else
                ClientPrint("Targeting cancelled.");
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

        public TargetInfo? AskForLocation() => targeting.LocationInfo();

        public void WaitForTarget(TimeSpan? timeout = null)
        {
            CheckCancellation();

            targeting.WaitForTarget(timeout ?? DefaultTimeout);
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
            if (WaitForItemDragged(timeout ?? DefaultTimeout) != DragResult.Success)
                return false;

            if (dropDelay.HasValue)
                Wait(dropDelay.Value);

            DropItem(item, targetContainerId);

            return true;
        }

        public DragResult WaitForItemDragged(TimeSpan? timeout = null)
        {
            return itemsObserver.WaitForItemDragged(timeout ?? DefaultTimeout);
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

        public void Wear(Item item, Layer layer, TimeSpan? timeout = null)
        {
            if (!TryWear(item, layer, timeout))
                throw new LegacyException($"Cannot pickup {item}");
        }

        public bool TryWear(Item item, Layer layer, TimeSpan? timeout = null)
        {
            DragItem(item, 1);
            if (WaitForItemDragged(timeout ?? DefaultTimeout) != DragResult.Success)
                return false;

            Server.Wear(item.Id, layer, Me.PlayerId);

            return true;
        }

        public void CastSpell(Spell spell)
        {
            NotifyAction();

            Server.CastSpell(spell);
        }

        public void UseSkill(Skill skill)
        {
            NotifyAction();

            Server.UseSkill(skill);
            eventJournalSource.Publish(new SkillRequestedEvent(skill));
        }

        public void OpenDoor()
        {
            Server.OpenDoor();
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

        public DialogBox WaitForDialogBox(params string[] failMessages)
            => WaitForDialogBox(false, null, failMessages);

        public DialogBox WaitForDialogBox(bool showDialog = false, params string[] failMessages)
            => WaitForDialogBox(showDialog, null, failMessages);

        public DialogBox WaitForDialogBox(bool showDialog = false, TimeSpan? timeout = null, params string[] failMessages)
        {
            DialogBox result = null;

            try
            {
                dialogBoxObervers.ShowDialogBox = showDialog;

                legacyEventJournal.When<DialogBoxOpenedEvent>(e => result = e.DialogBox)
                    .When<SpeechReceivedEvent>(e => failMessages.Any(msg => e.Speech.Text.Contains(msg)),
                        e => result = null)
                    .WaitAny(timeout);

                return result;
            }
            finally 
            {
                dialogBoxObervers.ShowDialogBox = true;
            }
        }

        public void TriggerDialogBox(string dialogResponse)
        {
            CheckCancellation();
            NotifyAction();

            dialogBoxObervers.TriggerDialogBox(dialogResponse);
        }

        public void TriggerDialogBox(byte responseIndex)
        {
            CheckCancellation();
            NotifyAction();

            dialogBoxObervers.TriggerDialogBox(responseIndex);
        }
    }
}