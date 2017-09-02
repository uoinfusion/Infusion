using System;
using System.Collections.Generic;
using System.Threading;
using Infusion.Commands;
using Infusion.Gumps;

namespace Infusion.LegacyApi
{
    // ReSharper disable once InconsistentNaming
    public static class UO
    {
        private static Legacy Current { get; set; }

        public static LegacyEvents Events => Current.Events;

        public static Configuration Configuration => Current.Configuration;

        public static Gump CurrentGump => Current.CurrentGump;

        public static CancellationToken? CancellationToken
        {
            get => Current.CancellationToken;
            set => Current.CancellationToken = value;
        }

        public static CommandHandler CommandHandler => Current.CommandHandler;

        public static UltimaMap Map => Current.Map;

        internal static GameObjectCollection GameObjects => Current.GameObjects;
        public static ItemCollection Items => Current.Items;
        public static MobileCollection Mobiles => Current.Mobiles;

        public static Player Me => Current.Me;

        public static GameJournal Journal => Current.Journal;

        public static IEnumerable<ObjectId> IgnoredObjects => Current.IgnoredItems;

        public static void Initialize(Legacy current)
        {
            Current = current;
        }

        public static void OpenContainer(Item container, TimeSpan? timeout = null)
            => Current.OpenContainer(container, timeout);

        public static void OpenContainer(ObjectId containerId, TimeSpan? timeout = null)
            => Current.OpenContainer(containerId, timeout);

        public static Command RegisterCommand(string name, Action commandAction)
            => Current.RegisterCommand(name, commandAction);

        public static Command RegisterCommand(string name, Action<string> commandAction)
            => Current.RegisterCommand(name, commandAction);

        public static void Alert(string message)
            => Current.Alert(message);

        public static GameJournal CreateJournal()
            => Current.CreateJournal();

        public static void Say(string message)
            => Current.Say(message);

        public static Gump WaitForGump(TimeSpan? timeout = null)
            => Current.WaitForGump(timeout);

        public static void Use(ObjectId objectId)
            => Current.Use(objectId);

        public static void RequestStatus(Mobile item)
            => Current.RequestStatus(item);

        public static Item AskForItem()
            => Current.AskForItem();

        public static Mobile AskForMobile()
            => Current.AskForMobile();

        public static void Use(GameObject item)
            => Current.Use(item);

        public static void Click(GameObject obj)
            => Current.Click(obj);

        public static void Click(ObjectId id)
            => Current.Click(id);

        public static bool TryUse(ItemSpec spec)
            => Current.TryUse(spec);

        public static void Use(ItemSpec spec)
            => Current.Use(spec);

        public static bool TryUse(ModelId type)
            => Current.TryUse(type);

        public static void Use(ModelId type)
            => Current.Use(type);

        public static bool TryUse(params ModelId[] types)
            => Current.TryUse(types);

        public static void Use(params ModelId[] types)
            => Current.TryUse(types);

        public static void Wait(int milliseconds)
            => Current.Wait(milliseconds);

        public static void Wait(TimeSpan span)
            => Current.Wait(span);

        public static void WaitToAvoidFastWalk(MovementType movementType)
            => Current.WaitToAvoidFastWalk(movementType);

        public static void WaitWalkAcknowledged()
            => Current.WaitWalkAcknowledged();

        public static void Walk(Direction direction, MovementType movementType = MovementType.Walk)
            => Current.Walk(direction, movementType);

        public static void WarModeOn()
            => Current.WarModeOn();

        public static void WarModeOff()
            => Current.WarModeOff();

        public static AttackResult TryAttack(Mobile target, TimeSpan? timeout = null)
            => Current.TryAttack(target, timeout);

        public static void TargetTile(string tileInfo)
            => Current.TargetTile(tileInfo);

        public static void Target(GameObject item)
            => Current.Target(item);

        public static void Target(Player player)
            => Current.Target(player);

        public static void Terminate(string parameters)
            => Current.Terminate(parameters);

        public static string Info()
            => Current.Info();

        public static void WaitForTarget()
            => Current.WaitForTarget();

        public static void DropItem(Item item, Item targetContainer)
            => Current.DropItem(item, targetContainer);

        public static void DropItem(Item item, ObjectId targetContainerId)
            => Current.DropItem(item, targetContainerId);

        public static void DropItem(ObjectId itemId, ObjectId targetContainerId)
            => Current.DropItem(itemId, targetContainerId);

        public static void DragItem(Item item)
            => Current.DragItem(item);

        public static void DragItem(Item item, ushort amount)
            => Current.DragItem(item, amount);

        public static bool TryMoveItem(Item item, Item targetContainer, TimeSpan? timeout = null,
            TimeSpan? dropDelay = null)
            => Current.TryMoveItem(item, targetContainer, timeout, dropDelay);

        public static bool TryMoveItem(Item item, ObjectId targetContainerId, TimeSpan? timeout = null,
            TimeSpan? dropDelay = null)
            => Current.TryMoveItem(item, targetContainerId, timeout, dropDelay);

        public static bool TryMoveItem(Item item, ushort amount, Item targetContainer, TimeSpan? timeout = null,
            TimeSpan? dropDelay = null)
            => Current.TryMoveItem(item, amount, targetContainer, timeout, dropDelay);

        public static bool TryMoveItem(Item item, ushort amount, ObjectId targetContainerId, TimeSpan? timeout = null,
            TimeSpan? dropDelay = null)
            => Current.TryMoveItem(item, amount, targetContainerId, timeout, dropDelay);

        public static DragResult WaitForItemDragged(TimeSpan? timeout = null)
            => Current.WaitForItemDragged(timeout);

        public static void Log(string message)
            => Current.Log(message);

        public static void TriggerGump(GumpControlId triggerId)
            => Current.TriggerGump(triggerId);

        public static GumpResponseBuilder GumpResponse()
            => Current.GumpResponse();

        public static void SelectGumpButton(string buttonLabel, GumpLabelPosition labelPosition)
            => Current.SelectGumpButton(buttonLabel, labelPosition);

        public static void LastGumpInfo()
            => Current.LastGumpInfo();

        public static void CloseGump()
            => Current.CloseGump();

        public static void StepToward(Location2D currentLocation, Location2D targetLocation)
            => Current.StepToward(currentLocation, targetLocation);

        public static void StepToward(GameObject gameObject)
            => Current.StepToward(gameObject);

        public static void StepToward(Location2D targetLocation)
            => Current.StepToward(targetLocation);

        public static void WalkTo(Location2D targetLocation)
            => Current.WalkTo(targetLocation);

        public static void WalkTo(ushort xloc, ushort yloc)
            => Current.WalkTo(xloc, yloc);

        public static bool TryWear(Item item, Layer layer, TimeSpan? timeout = null)
            => Current.TryWear(item, layer, timeout);

        public static void Wear(Item item, Layer layer, TimeSpan? timeout = null)
            => Current.Wear(item, layer, timeout);

        public static void CastSpell(Spell spell)
            => Current.CastSpell(spell);

        public static void UseSkill(Skill skill)
            => Current.UseSkill(skill);

        public static void OpenDoor()
            => Current.OpenDoor();

        public static void Ignore(Item item)
            => Current.Ignore(item);

        public static void ClientPrint(string message, string name, ObjectId itemId, ModelId itemModel, SpeechType type,
            Color color, bool log = true)
            => Current.ClientPrint(message, name, itemId, itemModel, type, color, log);

        public static void ClientPrint(string message, bool log = true)
            => Current.ClientPrint(message, log);

        public static void ClientPrint(string message, string name, Player onBehalfPlayer, bool log = true)
            => Current.ClientPrint(message, name, onBehalfPlayer, log);

        public static void ClientPrint(string message, string name, Item onBehalfItem, bool log = true)
            => Current.ClientPrint(message, name, onBehalfItem, log);

        public static void CloseContainer(Item container)
            => Current.CloseContainer(container);

        public static void ToggleLightFiltering() => Current.ToggleLightFiltering();

        public static void ToggleWeatherFiltering() => Current.ToggleWeatherFiltering();
    }
}