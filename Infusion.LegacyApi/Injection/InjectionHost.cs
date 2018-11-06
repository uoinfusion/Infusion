using Infusion.LegacyApi.Console;
using InjectionScript.Interpretation;
using InjectionScript.Parsing;
using System;

namespace Infusion.LegacyApi.Injection
{
    public sealed class InjectionHost
    {
        private readonly Runtime runtime;
        private readonly Legacy api;
        private readonly IConsole console;
        internal FindTypeSubrutine FindTypeSubrutine { get; }

        public InjectionHost(Legacy api, IConsole console)
        {
            this.api = api;
            this.console = console;

            runtime = new Runtime();

            this.FindTypeSubrutine = new FindTypeSubrutine(api, runtime);

            RegisterNatives();
        }

        public void LoadScript(string fileName)
        {
            try
            {
                runtime.Load(fileName);
            }
            catch (SyntaxErrorException ex)
            {
                foreach (var error in ex.Errors)
                    console.Error($"{error.Line}, {error.CharPos} {error.Message}");
            }
        }

        public void CallSubrutine(string subrutineName) => runtime.CallSubrutine(subrutineName);

        private void RegisterNatives()
        {
            runtime.Metadata.Add(new NativeSubrutineDefinition("wait", (Action<int>)Wait));

            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "set", (Action<string, string>)Set));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "set", (Action<string, int>)Set));

            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "print", (Action<string>)Print));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "getx", (Func<int>)GetX));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "getx", (Func<string, int>)GetX));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "getx", (Func<int, int>)GetX));

            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "gety", (Func<int>)GetY));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "gety", (Func<string, int>)GetY));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "gety", (Func<int, int>)GetY));

            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "getz", (Func<int>)GetZ));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "getz", (Func<string, int>)GetZ));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "getz", (Func<int, int>)GetZ));

            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "getdistance", (Func<string, int>)GetDistance));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "getdistance", (Func<int, int>)GetDistance));

            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "gethp", (Func<int>)GetHP));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "gethp", (Func<int, int>)GetHP));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "gethp", (Func<string, int>)GetHP));

            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "getmaxhp", (Func<int>)GetMaxHP));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "getmaxhp", (Func<int, int>)GetMaxHP));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "getmaxhp", (Func<string, int>)GetMaxHP));

            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "getnotoriety", (Func<int, int>)GetNotoriety));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "getnotoriety", (Func<string, int>)GetNotoriety));

            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "getname", (Func<int, string>)GetName));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "getname", (Func<string, string>)GetName));

            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "isnpc", (Func<int, int>)IsNpc));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "isnpc", (Func<string, int>)IsNpc));

            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "getserial", (Func<string, string>)GetSerial));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "getquantity", (Func<string, int>)GetQuantity));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "getquantity", (Func<int, int>)GetQuantity));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "dead", (Func<int>)Dead));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "hidden", (Func<int>)Hidden));

            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "str", (Func<int>)Str));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "int", (Func<int>)Int));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "dex", (Func<int>)Dex));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "stamina", (Func<int>)Stamina));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "mana", (Func<int>)Mana));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "weight", (Func<int>)Weight));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "gold", (Func<int>)Gold));

            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "findtype", (Action<string>)FindTypeSubrutine.FindType));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "findtype", (Action<int>)FindTypeSubrutine.FindType));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "findtype", (Action<int, int, int>)FindTypeSubrutine.FindType));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "findtype", (Action<string, string, string>)FindTypeSubrutine.FindType));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "findcount", (Func<int>)(() => FindTypeSubrutine.FindCount)));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "ignore", (Action<int>)FindTypeSubrutine.Ignore));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "ignore", (Action<string>)Ignore));

            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "click", (Action<string>)Click));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "useobject", (Action<string>)UseObject));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "useobject", (Action<int>)UseObject));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "attack", (Action<string>)Attack));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "attack", (Action<int>)Attack));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "getstatus", (Action<string>)GetStatus));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "getstatus", (Action<int>)GetStatus));

            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "say", (Action<string>)Say));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "msg", (Action<string>)Say));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "serverprint", (Action<string>)Say));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "click", (Action<string>)Click));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "click", (Action<string>)Click));

            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "warmode", (Action<int>)WarMode));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "warmode", (Func<int>)WarMode));

            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "useskill", (Action<string>)UseSkill));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "cast", (Action<string>)Cast));
        }

        public void Wait(int ms) => api.Wait(ms);

        public void Set(string name, int value)
        {
            if (name.Equals("finddistance", StringComparison.OrdinalIgnoreCase))
                FindTypeSubrutine.Distance = value;
        }

        public void Set(string name, string valueStr)
        {
            bool successfulConversion = NumberConversions.TryStr2Int(valueStr, out var value);

            if (name.Equals("finddistance", StringComparison.OrdinalIgnoreCase))
            {
                FindTypeSubrutine.Distance = successfulConversion ? value : 0;

            }
        }

        public void Print(string msg) => api.ClientPrint(msg);
        public int GetX() => api.Me.Location.X;
        public int GetX(string id) => GetX(GetObject(id));
        public int GetX(int id) => api.GameObjects[(uint)id]?.Location.X ?? 0;

        public int GetY() => api.Me.Location.Y;
        public int GetY(string id) => GetY(GetObject(id));
        public int GetY(int id) => api.GameObjects[(uint)id]?.Location.Y ?? 0;

        public int GetZ() => api.Me.Location.Z;
        public int GetZ(string id) => GetZ(GetObject(id));
        public int GetZ(int id) => api.GameObjects[(uint)id]?.Location.Z ?? 0;

        public int GetDistance(string id) => GetDistance(GetObject(id));
        public int GetDistance(int id)
        {
            var obj = api.GameObjects[(uint)id];

            return obj != null ? api.Me.GetDistance(obj) : 0;
        }

        public int GetHP() => api.Me.CurrentHealth;
        public int GetHP(string id) => GetHP(GetObject(id));
        public int GetHP(int id) => api.Mobiles[(uint)id]?.CurrentHealth ?? 0;

        public int GetMaxHP() => api.Me.MaxHealth;
        public int GetMaxHP(string id) => GetMaxHP(GetObject(id));
        public int GetMaxHP(int id) => api.Mobiles[(uint)id]?.MaxHealth ?? 0;

        public int GetNotoriety(string id) => GetNotoriety(GetObject(id));
        public int GetNotoriety(int id) => (int?)api.Mobiles[(uint)id]?.Notoriety ?? 0;

        public string GetName(string id) => GetName(GetObject(id));
        public string GetName(int id) => api.GameObjects[(uint)id]?.Name ?? string.Empty;

        public int IsNpc(string id) => IsNpc(GetObject(id));
        public int IsNpc(int id) => api.Mobiles[(uint)id] != null ? 1 : 0;

        public int GetQuantity(string id) => GetQuantity(GetObject(id));
        public int GetQuantity(int id)
        {
            var item = api.Items[(uint)id];
            if (item != null)
                return item.Amount;

            var corpse = api.Corpses[(uint)id];
            if (corpse != null)
                return corpse.CorpseType;

            return 0;
        }

        public string GetSerial(string id) => NumberConversions.Int2Hex(GetObject(id));
        public int Dead() => api.Me.IsDead ? 1 : 0;
        public int Hidden() => api.Me.IsHidden? 1 : 0;

        public int Str() => api.Me.Strength;
        public int Int() => api.Me.Intelligence;
        public int Dex() => api.Me.Dexterity;
        public int Stamina() => api.Me.CurrentStamina;
        public int Mana() => api.Me.CurrentMana;
        public int Weight() => api.Me.Weight;
        public int Gold() => (int)api.Me.Gold;

        public void Ignore(string id) => FindTypeSubrutine.Ignore(GetObject(id));
        public void Click(string id) => api.Click((uint)GetObject(id));
        public void UseObject(int id) => api.Use((uint)id);
        public void UseObject(string id) => api.Use((uint)GetObject(id));
        public void Attack(string id) => api.TryAttack((uint)GetObject(id));
        public void Attack(int id) => api.TryAttack((uint)id);
        public void GetStatus(string id) => api.RequestStatus((uint)GetObject(id));
        public void GetStatus(int id) => api.RequestStatus((uint)id);

        public void Say(string message) => api.Say(message);

        public void WarMode(int mode)
        {
            if (mode == 0)
                api.WarModeOff();
            else
                api.WarModeOn();
        }
        public int WarMode() => api.Me.IsInWarMode ? 1 : 0;

        public void UseSkill(string skillName) => api.UseSkill(TranslateSkill(skillName));
        public void Cast(string spellName) => api.CastSpell(TranslateSpell(spellName));

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
                default:
                    throw new NotImplementedException($"Unknown skill {skillName}");
            }
        }

        private int GetObject(string id)
        {
            if (id.Equals("finditem", StringComparison.OrdinalIgnoreCase))
                return FindTypeSubrutine.FindItem;
            if (id.Equals("self", StringComparison.OrdinalIgnoreCase))
                return (int)api.Me.PlayerId;

            return runtime.GetObject(id);
        }

        private Spell TranslateSpell(string spellName)
        {
            switch (spellName.ToLower())
            {
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
    }
}
