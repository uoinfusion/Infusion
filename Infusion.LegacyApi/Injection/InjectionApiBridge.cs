using Infusion.LegacyApi.Console;
using Infusion.Packets;
using InjectionScript.Runtime;
using System;

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

        public InjectionApiBridge(Legacy infusionApi, InjectionHost injectionHost, IConsole console,
            PacketDefinitionRegistry packetRegistry)
        {
            this.infusionApi = infusionApi;
            this.injectionHost = injectionHost;
            this.console = console;
            this.packetRegistry = packetRegistry;
            this.findType = new FindTypeSubrutine(infusionApi, injectionHost);
            this.journal = new Journal(1000);
            this.equipmentSubrutines = new EquipmentSubrutines(infusionApi);
            this.useSubrutines = new UseSubrutines(infusionApi);
            this.targeting = new Targeting(infusionApi, injectionHost);
            this.grabbing = new Grabbing(infusionApi, injectionHost);
            this.morphing = new Morphing(infusionApi, packetRegistry);
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

        public void FindType(int type, int color, int containerId) => findType.FindType(type, color, containerId);
        public int FindCount() => findType.FindCount;
        public int Count(int type, int color) => findType.Count(type, color);
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

        public void SetReceivingContainer(int id) => grabbing.SetReceivingContainer(id);
        public void UnsetReceivingContainer() => grabbing.UnsetReceivingContainer();
        public void Grab(int amount, int id) => grabbing.Grab(amount, id);
        public void MoveItem(int id, int amount, int targetContainerId)
            => grabbing.MoveItem(id, amount, targetContainerId);

        public void LClick(int x, int y) => infusionApi.ClientWindow.Click(x, y);
        public void KeyPress(int key) => infusionApi.ClientWindow.PressKey((KeyCode)key);
        public void Say(string message) => infusionApi.ClientWindow.SendText(message);

        public void PlayWav(string file) => WavPlayer.Play(file);

        public void TextOpen() { /* just do nothing */ }
        public void TextPrint(string text) => infusionApi.Log(text);

        public void ServerPrint(string message) => infusionApi.Say(message);
        public void Print(string msg) => infusionApi.ClientPrint(msg);

        public int InJournal(string pattern) => journal.InJournal(pattern);
        public void DeleteJournal() => journal.DeleteJournal();
        public void DeleteJournal(string text) => journal.DeleteJournal(text);
        public string GetJournalText(int index) => journal.GetJournalText(index);
        public string JournalSerial(int index) => journal.JournalSerial(index);
        public string JournalColor(int index) => journal.JournalColor(index);
        public void SetJournalLine(int index) => journal.SetJournalLine(index);
        public void SetJournalLine(int index, string text) => journal.SetJournalLine(index);

        public void Arm(string name) => equipmentSubrutines.Arm(name);
        public void SetArm(string name) => equipmentSubrutines.SetArm(name);
        public void Unequip(string layer) => equipmentSubrutines.Unequip(layer);
        public void Equip(string layer, int id) => equipmentSubrutines.Equip(layer, id);
        public int ObjAtLayer(string layer) => equipmentSubrutines.ObjAtLayer(layer);
        public void WaitTargetObject(int id) => targeting.WaitTargetObject((ObjectId)id);
        public void WaitTargetObject(int id1, int id2) => targeting.WaitTargetObject((ObjectId)id1, (ObjectId)id2);
        public void WaitTargetTile(int type, int x, int y, int z) => targeting.WaitTargetTile(type, x, y, z);
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

        public void UseSkill(string skillName) => infusionApi.UseSkill(TranslateSkill(skillName));
        public void Cast(string spellName, int id) => infusionApi.CastSpell(TranslateSpell(spellName));
        public void Cast(string spellName) => throw new NotImplementedException();

        public void Morph(int type) => morphing.Morph(type);

        private Skill TranslateSkill(string skillName)
        {
            switch (skillName.ToLower())
            {
                case "animal lore":
                    return Skill.AnimalLore;
                case "animal taming":
                    return Skill.AnimalTaming;
                case "hiding":
                    return Skill.Hiding;
                case "peacemaking":
                    return Skill.Peacemaking;
                case "meditation":
                    return Skill.Meditation;
                case "tracking":
                    return Skill.Tracking;
                case "hid":
                    return Skill.DetectingHidden;
                default:
                    throw new NotImplementedException($"Unknown skill {skillName}");
            }
        }

        private Spell TranslateSpell(string spellName)
        {
            switch (spellName.ToLower())
            {
                case "harm":
                    return Spell.Harm;
                case "bless":
                    return Spell.Bless;
                case "protection":
                    return Spell.Protection;
                case "magic reflection":
                    return Spell.Reflection;
                case "reactive armor":
                    return Spell.ReactiveArmor;
                case "agility":
                    return Spell.Agility;
                case "strength":
                    return Spell.Strength;
                case "cunning":
                    return Spell.Cunning;
                case "night sight":
                    return Spell.NightSight;
                case "recall":
                    return Spell.Recall;
                case "arch protection":
                    return Spell.ArchProtection;
                default:
                    throw new NotImplementedException($"Unknown spell {spellName}");
            }
        }

        public void Terminate(string subrutineName) => injectionHost.Terminate(subrutineName);
        public void Error(string message) => console.Error(message);
    }
}
