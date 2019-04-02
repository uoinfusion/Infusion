using Infusion.LegacyApi.Console;
using Infusion.Packets;
using InjectionScript.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infusion.LegacyApi.Injection
{
    internal sealed class InjectionApiBridge : IApiBridge
    {
        private readonly Legacy infusionApi;
        private readonly InjectionHost injectionHost;
        private readonly IConsole console;
        private readonly PacketDefinitionRegistry packetRegistry;
        private readonly FindTypeSubrutine findType;
        private readonly ItemObservers itemObservers;
        private readonly Targeting targeting;
        private readonly UseSubrutines useSubrutines;
        private readonly Grabbing grabbing;
        private readonly Journal journal;
        private readonly EquipmentSubrutines equipmentSubrutines;
        private readonly Morphing morphing;
        private readonly WavPlayer wavPlayer;
        private readonly GumpSubrutines gumps;
        private readonly ObjectNameReceiver objectNameReceiver;
        private readonly Walker walker;
        private readonly Menus menus;

        public InjectionApiBridge(Legacy infusionApi, InjectionHost injectionHost, IConsole console,
            PacketDefinitionRegistry packetRegistry)
        {
            this.infusionApi = infusionApi;
            this.injectionHost = injectionHost;
            this.console = console;
            this.packetRegistry = packetRegistry;
            this.findType = new FindTypeSubrutine(infusionApi, injectionHost);
            this.journal = new Journal(1000, () => injectionHost.InjectionApi.Now());
            this.equipmentSubrutines = new EquipmentSubrutines(infusionApi);
            this.useSubrutines = new UseSubrutines(infusionApi);
            this.targeting = new Targeting(infusionApi, injectionHost, infusionApi.Client, infusionApi.Targeting);
            this.grabbing = new Grabbing(infusionApi, injectionHost);
            this.morphing = new Morphing(infusionApi, packetRegistry);
            this.wavPlayer = new WavPlayer(console);
            this.gumps = new GumpSubrutines(infusionApi, infusionApi.GumpObservers, console);
            this.objectNameReceiver = new ObjectNameReceiver(infusionApi);
            this.walker = new Walker(infusionApi);
            this.menus = new Menus(infusionApi.DialogBoxObservers);

            infusionApi.JournalSource.NewMessageReceived += (sender, entry) => journal.Add(entry);

            itemObservers = new ItemObservers(infusionApi.Server, infusionApi.Client, packetRegistry);
        }

        public int FindItem => findType.FindItem;
        public int Self => (int)infusionApi.Me.PlayerId;
        public int LastCorpse => (int)itemObservers.LastCorpseId;
        public int LastStatus => (int)itemObservers.LastStatusId;
        public int LastTarget => (int)itemObservers.LastTargetId;
        public int Backpack => (int)infusionApi.Me.BackPack.Id;

        public int Strength => infusionApi.Me.Strength;

        public int Intelligence => infusionApi.Me.Intelligence;

        public int Dexterity => infusionApi.Me.Dexterity;

        public int Stamina => infusionApi.Me.CurrentStamina;

        public int Mana => infusionApi.Me.CurrentMana;

        public int Weight => infusionApi.Me.Weight;

        public int Gold => (int)infusionApi.Me.Gold;

        public int GetX(int id) => infusionApi.GameObjects[(ObjectId)id]?.Location.X ?? 0;
        public int GetY(int id) => infusionApi.GameObjects[(ObjectId)id]?.Location.Y ?? 0;
        public int GetZ(int id) => infusionApi.GameObjects[(ObjectId)id]?.Location.Z ?? 0;

        public void SetFindDistance(int distance) => findType.Distance = distance;
        public void SetGrabDelay(int delay) => grabbing.SetGrabDelay(delay);

        public void Wait(int ms) => infusionApi.Wait(ms);

        public int GetDistance(int id)
        {
            var obj = infusionApi.GameObjects[(uint)id];

            return obj != null ? infusionApi.Me.GetDistance(obj) : 0;
        }

        public int GetHP(int id) => infusionApi.Mobiles[(uint)id]?.CurrentHealth ?? 0;
        public int GetMaxHP(int id) => infusionApi.Mobiles[(uint)id]?.MaxHealth ?? 0;
        public int GetNotoriety(int id) => (int?)infusionApi.Mobiles[(uint)id]?.Notoriety ?? 0;
        public string GetName(int id) => infusionApi.GameObjects[(uint)id]?.Name ?? string.Empty;
        public int GetGraphics(int id) => infusionApi.GameObjects[(uint)id]?.Type ?? 0;
        public int GetDir(int id) => (int)(infusionApi.Mobiles[(uint)id]?.Orientation ?? Direction.North);
        public int IsNpc(int id) => infusionApi.Mobiles[(uint)id] != null ? 1 : 0;
        public int GetColor(int id) => infusionApi.Mobiles[(uint)id]?.Color ?? infusionApi.Items[(uint)id]?.Color ?? 0;
        public int GetLayer(int id) => (int)(infusionApi.Items[(uint)id]?.Layer ?? 0);
        public int ContainerOf(int id)
        {
            var containerId = infusionApi.Items[(uint)id]?.ContainerId;
            if (containerId.HasValue)
                return (int)containerId;

            return -1;
        }

        public int GetQuantity(int id)
        {
            var item = infusionApi.Items[(uint)id];
            if (item != null)
                return item.Amount;

            var corpse = infusionApi.Corpses[(uint)id];
            if (corpse != null)
                return corpse.CorpseType;

            return 0;
        }

        public int Exists(int id) => infusionApi.GameObjects[(uint)id] == null ? 0 : 1;
        public int IsOnline() => infusionApi.Me == null || infusionApi.Me.PlayerId == 0 ? 0 : 1;
        public int Dead() => infusionApi.Me.IsDead ? 1 : 0;
        public int Hidden(int id)
        {
            if (id == infusionApi.Me.PlayerId)
                return infusionApi.Me.IsHidden ? 1 : 0;

            return 0;
        }

        public int FindType(int type, int color, int containerId, int range, bool recursive) 
            => findType.FindType(type, color, containerId, range, recursive);
        public int FindCount() => findType.FindCount;
        public int Count(int type, int color, int containerId) => findType.Count(type, color, containerId);
        public void Ignore(int id) => findType.Ignore(id);
        public void IgnoreReset() => findType.IgnoreReset();

        public void AddObject(string name) => targeting.AddObject(name);

        public int Str() => infusionApi.Me.Strength;
        public int Int() => infusionApi.Me.Intelligence;
        public int Dex() => infusionApi.Me.Dexterity;

        public void Click(int id) => infusionApi.Click((uint)id);
        public void UseObject(int id) => infusionApi.Use((uint)id);
        public void Attack(int id) => infusionApi.TryAttack((uint)id);
        public void GetStatus(int id) => infusionApi.RequestStatus((uint)id);

        public void UseType(int type, int color) => useSubrutines.UseType(type, color);

        public int IsTargeting() => targeting.IsTargeting ? 1 : 0;

        public int[] LastTile() => targeting.LastTile();

        public void SetReceivingContainer(int id) => grabbing.SetReceivingContainer(id);
        public void UnsetReceivingContainer() => grabbing.UnsetReceivingContainer();
        public void Grab(int amount, int id) => grabbing.Grab(amount, id);
        public void MoveItem(int id, int amount, int targetContainerId)
            => grabbing.MoveItem(id, amount, targetContainerId);
        public void MoveItem(int id, int amount, int targetContainerId, int x, int y, int z)
            => grabbing.MoveItem(id, amount, targetContainerId, x, y, z);

        public void LClick(int x, int y) => infusionApi.ClientWindow.Click(x, y);
        public void KeyPress(int key) => infusionApi.ClientWindow.PressKey((KeyCode)key);
        public void Say(string message) => infusionApi.ClientWindow.SendText(message);

        public void PlayWav(string file) => wavPlayer.Play(file);

        public void TextOpen() { /* just do nothing */ }
        public void TextPrint(string text) => infusionApi.Log(text);
        public void TextClear() => infusionApi.CommandHandler.Invoke("cls");

        public void ServerPrint(string message) => infusionApi.Say(message);
        public void Print(string msg) => infusionApi.ClientPrint(msg);
        public void Print(int color, string msg) => infusionApi.ClientPrint(msg, new Color((ushort)color));

        public int InJournal(string pattern) => journal.InJournal(pattern);
        public int InJournalBetweenTimes(string pattern, int startTime, int endTime, int limit) 
            => journal.InJournalBetweenTime(pattern, startTime, endTime, limit);
        public void DeleteJournal() => journal.DeleteJournal();
        public void DeleteJournal(string text) => journal.DeleteJournal(text);
        public string GetJournalText(int index) => journal.GetJournalText(index);
        public string JournalSerial(int index) => journal.JournalSerial(index);
        public string JournalColor(int index) => journal.JournalColor(index);
        public void SetJournalLine(int index) => journal.SetJournalLine(index, null);
        public void SetJournalLine(int index, string text) => journal.SetJournalLine(index, text);

        public void Arm(string name) => equipmentSubrutines.Arm(name);
        public void SetArm(string name) => equipmentSubrutines.SetArm(name);
        public void Unequip(int layer) => equipmentSubrutines.Unequip(layer);
        public void Equip(int layer, int id) => equipmentSubrutines.Equip(layer, id);
        public int ObjAtLayer(int layer) => equipmentSubrutines.ObjAtLayer(layer);
        public void WaitTargetObject(int id) => targeting.WaitTargetObject((ObjectId)id);
        public void WaitTargetObject(int id1, int id2) => targeting.WaitTargetObject((ObjectId)id1, (ObjectId)id2);
        public void WaitTargetTile(int type, int x, int y, int z) => targeting.WaitTargetTile(type, x, y, z);
        public void CancelNextTarget() => targeting.CancelNextTarget();
        public void CancelTarget() => targeting.CancelTarget();
        public int[] GetWaitTargetQueue() => targeting.GetWaitTargetQueue();

        public void CharPrint(int id, int color, string msg)
        {
            var objectId = (ObjectId)id;
            var name = infusionApi.GameObjects[(ObjectId)id]?.Name ?? "injection";

            infusionApi.ClientPrint(msg, name, objectId, 0, SpeechType.Normal, (Color)color);
        }

        public void WarMode(int mode)
        {
            if (mode == 0)
                infusionApi.WarModeOff();
            else
                infusionApi.WarModeOn();
        }

        public int WarMode() => infusionApi.Me.IsInWarMode ? 1 : 0;

        public void UseSkill(int skillId) => infusionApi.UseSkill((Skill)skillId);
        public int SkillVal(int skillId) => infusionApi.Me.Skills[(Skill)skillId].Value;
        public void Cast(int spellId) => infusionApi.CastSpell((Spell)spellId);

        public void Morph(int type) => morphing.Morph(type);
        public void MakeStepByKey(int key) => walker.MakeStepByKey(key);
        public void EnableMove(bool enabled)
        {
            if (enabled)
                infusionApi.ClientFilters.Walking.Enable();
            else
                infusionApi.ClientFilters.Walking.Disable();
        }

        public string PrivateGetTile(int x, int y, int unknown, int tileMin, int tileMax)
        {
            var tiles = Ultima.Map.Felucca.Tiles.GetStaticTiles(x, y);
            var tileIds = tiles.Where(tile => tile.ID >= tileMin && tile.ID <= tileMax)
                .Select(tile => $"0x{tile.ID:X4}");
            if (tileIds.Any())
                return tileIds.Aggregate((l, r) => l + "," + r);

            return string.Empty;
        }

        public void ReceiveObjectName(int id, int timeout) => objectNameReceiver.Receive(id, timeout);

        public void Snap(string name)
        {
            if (string.IsNullOrEmpty(name))
                Screenshot.Snap();
            else
                Screenshot.Snap(name);
        }

        public void WaitGump(int triggerId) => gumps.WaitGump(triggerId);
        public void SendGumpSelect(int triggerId) => gumps.SendGumpSelect(triggerId);

        public void WaitMenu(params string[] parameters) => menus.WaitMenu(parameters);

        public void Terminate(string subrutineName) => injectionHost.Terminate(subrutineName);
        public void Error(string message) => console.Error(message);
        public void Exec(string subrutineName) => injectionHost.ExecSubrutine(subrutineName);

        public bool FunRunning(string subrutineName) => injectionHost.FunRunning(subrutineName);

        public void Track(int x, int y) => infusionApi.Client.ShowQuestArrow(x, y);
        public void TrackOff() => infusionApi.Client.CancelQuest();
    }
}
