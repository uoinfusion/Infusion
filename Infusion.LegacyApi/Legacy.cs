using System;
using System.Linq;
using System.Threading;
using Infusion.Commands;
using Infusion.Config;
using Infusion.Gumps;
using Infusion.LegacyApi.Cliloc;
using Infusion.LegacyApi.Console;
using Infusion.LegacyApi.Events;
using Infusion.LegacyApi.Filters;
using Infusion.LegacyApi.Injection;
using Infusion.LegacyApi.Keywords;
using Infusion.Logging;
using Infusion.Packets;
using Infusion.Packets.Server;
using InjectionScript.Runtime;

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

        internal SpeechJournalSource JournalSource { get; }
        private readonly LightObserver lightObserver;

        private readonly IConsole console;
        private readonly PlayerObservers playerObservers;
        private readonly MapObserver mapObserver;
        private readonly SoundObserver soundObserver;
        private readonly ShapeshiftingFilter shapeShifter;
        private readonly WalkingObserver walkingObserver;

        internal Targeting Targeting { get; }
        private readonly WeatherObserver weatherObserver;
        private readonly Cancellation cancellation;

        private readonly EventJournalSource eventJournalSource;
        private readonly DialogBoxObservers dialogBoxObervers;
        private readonly ServerObservers serverObservers;

        internal AutoResetEvent WaitForTargetStartedEvent => Targeting.WaitForTargetStartedEvent;
        internal AutoResetEvent AskForTargetStartedEvent => Targeting.AskForTargetStartedEvent;
        internal AutoResetEvent WaitForGumpStartedEvent => gumpObservers.WaitForGumpStartedEvent;
        internal AutoResetEvent WaitForItemDraggedStartedEvent => itemsObserver.WaitForItemDraggedStartedEvent;

        internal GumpObservers GumpObservers => this.gumpObservers;
        internal DialogBoxObservers DialogBoxObservers => this.dialogBoxObervers;

        internal event Action LoginConfirmed;

        internal event Action<TargetInfo?> TargetInfoReceived
        {
            add => Targeting.TargetInfoReceived += value;
            remove => Targeting.TargetInfoReceived -= value;
        }

        internal Legacy(LogConfiguration logConfig, CommandHandler commandHandler,
            UltimaServer ultimaServer, UltimaClient ultimaClient, IConsole console, PacketDefinitionRegistry packetRegistry,
            IConfigBagRepository configRepository, IInjectionWindow injectionWindow, ISoundPlayer soundPlayer)
            : this(logConfig, commandHandler, ultimaServer, ultimaClient, console, packetRegistry,
                  new RealTimeSource(), new MulClilocSource(), new MulKeywordSource(), configRepository, injectionWindow, soundPlayer)
        {

        }

        internal Legacy(LogConfiguration logConfig, CommandHandler commandHandler,
            UltimaServer ultimaServer, UltimaClient ultimaClient, IConsole console, PacketDefinitionRegistry packetRegistry,
            ITimeSource timeSource, IClilocSource clilocSource, IKeywordSource keywordSource, IConfigBagRepository configRepository, IInjectionWindow injectionWindow,
            ISoundPlayer soundPlayer)
        {
            this.console = console;

            Trace = new GlobalTrace(console);

            cancellation = new Cancellation(() => CancellationToken);
            eventJournalSource = new EventJournalSource();
            Me = new Player(() => GameObjects.OfType<Item>().OnLayer(Layer.Mount).FirstOrDefault() != null,
                ultimaServer, this, eventJournalSource);
            gumpObservers = new GumpObservers(ultimaServer, ultimaClient, eventJournalSource, cancellation, packetRegistry);
            GameObjects = new GameObjectCollection(Me);
            Items = new ItemCollection(GameObjects);
            Mobiles = new MobileCollection(GameObjects);
            Corpses = new CorpseCollection(GameObjects);
            itemsObserver = new ItemsObservers(GameObjects, ultimaServer, ultimaClient, this, eventJournalSource);
            Me.LocationChanged += itemsObserver.OnPlayerPositionChanged;
            JournalSource = new SpeechJournalSource();
            JournalSource.NewMessageReceived +=
                (sender, entry) => eventJournalSource.Publish(new SpeechReceivedEvent(entry));
            Journal = new SpeechJournal(JournalSource, cancellation, () => DefaultTimeout, Trace.JournalTrace);
            journalObservers = new JournalObservers(JournalSource, ultimaServer, console, clilocSource);
            Targeting = new Targeting(ultimaServer, ultimaClient, cancellation, eventJournalSource, packetRegistry);

            blockedPacketsFilters = new BlockedClientPacketsFilters(ultimaClient);
            lightObserver = new LightObserver(ultimaServer, ultimaClient, Me, this, packetRegistry);
            weatherObserver = new WeatherObserver(ultimaServer, ultimaClient, this);
            soundObserver = new SoundObserver(ultimaServer, eventJournalSource, packetRegistry);
            mapObserver = new MapObserver(ultimaServer, eventJournalSource);
            shapeShifter = new ShapeshiftingFilter(ultimaServer, ultimaClient, packetRegistry);
            walkingObserver = new WalkingObserver(ultimaClient, ultimaClient, Me);
            var speechRequestObserver = new SpeechRequestObserver(ultimaClient, commandHandler, eventJournalSource, console, packetRegistry);
            var staminaFilter = new StaminaFilter(ultimaServer, ultimaClient);
            dialogBoxObervers = new DialogBoxObservers(ultimaServer, eventJournalSource, packetRegistry);

            playerObservers = new PlayerObservers(Me, ultimaClient, ultimaServer, console, this, GameObjects, eventJournalSource, packetRegistry);
            playerObservers.WalkRequestDequeued += Me.OnWalkRequestDequeued;

            Server = ultimaServer;
            Client = ultimaClient;

            CommandHandler = commandHandler;
            CommandHandler.CancellationTokenCreated += (sender, token) => CancellationToken = token;

            LogConfig = logConfig;
            legacyEventJournal = CreateEventJournal();

            ClientFilters = new LegacyFilters(staminaFilter, lightObserver, weatherObserver, soundObserver, shapeShifter, walkingObserver);
            RegisterDefaultCommands();

            Phantoms = new Phantoms(Client, Server, packetRegistry);

            Config = new ConfigBag(configRepository);
            Injection = new InjectionHost(this, injectionWindow, console, packetRegistry, timeSource, clilocSource, soundPlayer);

            serverObservers = new ServerObservers(ultimaServer, ultimaClient, packetRegistry);
            playerObservers.LoginConfirmed += () =>
            {
                IsLoginConfirmed = true;
                LoginConfirmed?.Invoke();
            };

            Chat = new Chat(ultimaServer, NotifyAction, console, keywordSource);
        }

        public Chat Chat { get; }

        public ConfigBag Config { get; }

        public string ServerName => serverObservers.SelectedServer.Name;
        public string SelectedCharacterName => serverObservers.SelectedCharacterName;

        public Phantoms Phantoms { get; }

        public InjectionHost Injection { get; }

        public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(30);

        public LogConfiguration LogConfig { get; }

        public LegacyFilters ClientFilters { get; }

        public GlobalTrace Trace { get; }

        public bool IsLoginConfirmed { get; internal set; }

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
        public CorpseCollection Corpses { get; }
        public IConsole Console => console;

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

        private void RegisterDefaultCommands()
        {
            CommandHandler.RegisterCommand(new Command("info", InfoCommand, true, true,
                "Shows information about selected item or tile."));
            CommandHandler.RegisterCommand(new Command("lastgumpinfo", LastGumpInfo, true, true,
                "Shows information about the last gump dispalyed in Ultima Online client."));
            CommandHandler.RegisterCommand(new Command("opendoor", OpenDoor, true, true,
                "Opens neares closed doors. This is analogue to UO client's 'opendoor' macro."));
            CommandHandler.RegisterCommand(new Command("warmode-on", WarModeOn, true, true,
                summary: "War mode on."));
            CommandHandler.RegisterCommand(new Command("warmode-off", WarModeOff, true, true,
                summary: "War mode off."));
            CommandHandler.RegisterCommand(new Command("terminate", Terminate, false, true,
                summary: "Terminates all running commands excluding background commands.", executionMode: CommandExecutionMode.Direct));
            CommandHandler.RegisterCommand(new Command("terminate-all", CommandHandler.TerminateAll, false, true,
                summary: "Terminates all running commands.", executionMode: CommandExecutionMode.Direct));
            CommandHandler.RegisterCommand(new Command("filter-light", ClientFilters.Light.Toggle, true, true));
            CommandHandler.RegisterCommand(new Command("filter-weather", ClientFilters.Weather.Toggle, true, true));
        }

        public SpeechJournal CreateSpeechJournal() => new SpeechJournal(JournalSource, cancellation, () => DefaultTimeout, Trace.JournalTrace);
        public EventJournal CreateEventJournal() => new EventJournal(eventJournalSource, cancellation, () => DefaultTimeout);

        public void Say(string message) => Chat.Say(message);

        public void NotifyAction()
        {
            NotifyAction(DateTime.UtcNow);
        }

        internal void NotifyAction(DateTime actionTime)
        {
            JournalSource.NotifyAction();
            eventJournalSource.NotifyAction();
            Targeting.NotifyLastAction(actionTime);
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

        internal void ClientPrint(string message, Player player, Color color, bool logMessage = true)
            => ClientPrint(message, player.Name ?? "<player>", player.PlayerId, player.BodyType, SpeechType.Speech, color, logMessage);

        internal void ClientPrint(string message, Mobile mobile, Color color, bool logMessage = true) 
            => ClientPrint(message, mobile.Name ?? "<mobile>", mobile.Id, mobile.Type, SpeechType.Speech, color, logMessage);

        internal void ClientPrint(string message, Item item, Color color, bool logMessage = true) 
            => ClientPrint(message, item.Name ?? "<item>", item.Id, item.Type, SpeechType.Speech, color, logMessage);

        internal void ClientPrint(string message, Player player, bool logMessage = true)
            => ClientPrint(message, player.Name ?? "<player>", player, logMessage);

        internal void ClientPrint(string message, Mobile mobile, bool logMessage = true) 
            => ClientPrint(message, mobile.Name ?? "<mobile>", mobile, logMessage);

        internal void ClientPrint(string message, Item item, bool logMessage = true)
            => ClientPrint(message, item.Name ?? "<item>", item, logMessage);

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
                       ?? Items.Matching(spec).InBackPack(true).FirstOrDefault()
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
                throw new LegacyException($"Cannot use requested item because you cannot see any such item, or no such item is in your backpack.");
        }

        public bool TryUse(ModelId type, Color? color = null)
        {
            CheckCancellation();

            var items = Items.OfType(type);
            if (color.HasValue)
                items = items.OfColor(color.Value);

            var item = items.OnLayer(Layer.OneHandedWeapon)
                           .FirstOrDefault(i => i.ContainerId.HasValue && i.ContainerId == Me.PlayerId)
                       ?? items.OnLayer(Layer.TwoHandedWeapon)
                           .FirstOrDefault(i => i.ContainerId.HasValue && i.ContainerId == Me.PlayerId)
                       ?? items.InBackPack(true).FirstOrDefault()
                       ?? items.OnLayer(Layer.Backpack)
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

            var item = Items.OfType(types).InBackPack(true)
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
                Thread.Sleep(25);
                milliseconds -= 25;
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

        public AttackResult TryAttack(ObjectId id, TimeSpan? timeout = null)
        {
            return playerObservers.Attack(id, timeout);
        }

        public void TargetTile(string tileInfo)
        {
            CheckCancellation();

            NotifyAction();
            Targeting.TargetTile(tileInfo);
        }

        public void Target(Location2D location)
        {
            CheckCancellation();
            NotifyAction();

            Targeting.TargetTile(location.X, location.Y, 0, 0);
        }

        public void Target(Location3D location)
        {
            CheckCancellation();
            NotifyAction();

            Targeting.TargetTile(location.X, location.Y, location.Z, 0);
        }

        public void Target(TargetInfo targetInfo)
        {
            CheckCancellation();

            NotifyAction();
            Targeting.Target(targetInfo);
        }

        public void Target(GameObject item)
        {
            CheckCancellation();

            NotifyAction();
            Targeting.Target(item);
        }

        public void Target(Player player)
        {
            CheckCancellation();

            NotifyAction();
            Targeting.Target(player);
        }

        public void Target(ObjectId id)
        {
            CheckCancellation();
            NotifyAction();

            var gameObject = GameObjects[id];
            if (gameObject == null)
                Targeting.Target(id);
            else
                Targeting.Target(gameObject);
        }

        public void Terminate(string parameters)
        {
            try
            {
                Log("Terminate attempt");
                if (string.IsNullOrEmpty(parameters))
                    CommandHandler.BeginTerminate();
                else
                    CommandHandler.Terminate(parameters);
            }
            finally
            {
                Log("Terminate attempt finished");
            }
        }

        public TargetInfo? Info() => Targeting.Info();
        internal void AskForTarget() => Targeting.AskForTarget();

        private void InfoCommand()
        {
            ClientPrint("Target item or tile to get info");
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
            var itemId = Targeting.ItemIdInfo();

            if (!itemId.HasValue || !GameObjects.TryGet(itemId.Value, out var obj))
                return null;

            return obj as Item;
        }

        public Mobile AskForMobile()
        {
            var itemId = Targeting.ItemIdInfo();

            if (!itemId.HasValue || !GameObjects.TryGet(itemId.Value, out var obj))
                return null;

            return obj as Mobile;
        }

        public TargetInfo? AskForLocation() => Targeting.LocationInfo();

        public bool WaitForTarget(params string[] failMessages)
            => WaitForTarget(DefaultTimeout, failMessages);

        public bool WaitForTarget(TimeSpan? timeout, params string[] failMessages)
        {
            CheckCancellation();

            return Targeting.WaitForTarget(timeout ?? DefaultTimeout, failMessages);
        }

        public void DropItem(Item item, Item targetContainer) => DropItem(item.Id, targetContainer.Id);
        public void DropItem(Item item, ObjectId targetContainerId) => DropItem(item.Id, targetContainerId);
        public void DropItem(Item item, Location3D location) => DropItem(item.Id, location);

        public void DropItem(ObjectId itemId, ObjectId targetContainerId)
        {
            CheckCancellation();

            NotifyAction();
            Server.DropItem(itemId, targetContainerId);
        }

        public void DropItem(ObjectId itemId, ObjectId targetContainerId, Location2D targetContainerLocation)
        {
            CheckCancellation();

            NotifyAction();
            Server.DropItem(itemId, targetContainerId, targetContainerLocation);
        }

        public void DropItem(ObjectId itemId, Location3D targetLocation)
        {
            CheckCancellation();

            NotifyAction();
            Server.DropItem(itemId, targetLocation);
        }

        public void DragItem(Item item) => DragItem(item.Id, item.Amount);
        public void DragItem(Item item, int amount) => DragItem(item.Id, amount);
        public void DragItem(ObjectId itemId) => DragItem(itemId, 0);

        public void DragItem(ObjectId itemId, int amount)
        {
            CheckCancellation();

            NotifyAction();

            itemsObserver.DragItem(itemId, amount);
        }

        public DragResult WaitForItemDragged(ObjectId? awaitedDragObjectId = null, TimeSpan? timeout = null)
        {
            return itemsObserver.WaitForItemDragged(awaitedDragObjectId, timeout ?? DefaultTimeout);
        }

        public void Log(string message)
        {
            CheckCancellation();

            console.Info(message);
        }

        public void TriggerGump(GumpControlId triggerId)
        {
            NotifyAction();

            gumpObservers.TriggerGump(triggerId);
        }

        public GumpResponseBuilder GumpResponse()
        {
            return gumpObservers.GumpResponse();
        }

        public void SelectGumpButton(string buttonLabel, GumpLabelPosition labelPosition)
        {
            NotifyAction();

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

        public void SellItems(ObjectId vendor, SellListItem[] items)
        {
            NotifyAction();
            Server.SellItems(vendor, items);
        }

        public void Wear(ObjectId id, Layer layer)
        {
            NotifyAction();

            Server.Wear(id, layer, Me.PlayerId);
        }

        public void Wear(Item item, Layer layer)
        {
            Wear(item.Id, layer);
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
            NotifyAction();

            Server.OpenDoor();
        }

        public void ClientPrint(string message, string name, ObjectId itemId, ModelId itemModel, SpeechType type,
            Color color, bool log = true)
        {
            Client.SendSpeech(message, name, itemId, itemModel, type, color);
            if (log)
                console.WriteSpeech(name, message, itemId, color, itemModel, type);
        }

        public void ClientPrint(string message, bool log = true)
        {
            ClientPrint(message, "System", new ObjectId(0), 0, SpeechType.Normal, (Color)0x03B2, log);
        }

        public void ClientPrint(string message, Color color, bool log = true)
        {
            ClientPrint(message, "System", new ObjectId(0), 0, SpeechType.Normal, color, log);
        }

        public void ClientPrint(string message, string name, Player onBehalfPlayer, bool log = true)
        {
            ClientPrint(message, name, onBehalfPlayer.PlayerId, onBehalfPlayer.BodyType, SpeechType.Speech,
                (Color)0x0026, log);
        }

        public void ClientPrint(string message, string name, Mobile onBehalf, bool log = true)
        {
            ClientPrint(message, name, onBehalf.Id, onBehalf.Type, SpeechType.Speech,
                (Color)0x0026, log);
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

        public bool TriggerDialogBox(string dialogResponse)
        {
            NotifyAction();

            return dialogBoxObervers.TriggerDialogBox(dialogResponse);
        }

        public bool TriggerDialogBox(byte responseIndex)
        {
            NotifyAction();

            return dialogBoxObervers.TriggerDialogBox(responseIndex);
        }

        public void CloseDialogBox()
        {
            dialogBoxObervers.CloseDialogBox();
        }

        public void WaitTargetObject(params ObjectId[] id) => Targeting.AddNextTarget(id);
        public void WaitTargetObject(params Item[] item) => Targeting.AddNextTarget(item);
        public void WaitTargetObject(params Mobile[] mobile) => Targeting.AddNextTarget(mobile);
        public void WaitTargetObject(Player player) => Targeting.AddNextTarget(player);
        public void WaitTargetTile(int type, int x, int y, int z) => Targeting.AddNextTarget(type, x, y, z);

        public void ClearTargetObject() => Targeting.ClearNextTarget();
    }
}