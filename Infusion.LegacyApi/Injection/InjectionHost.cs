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
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "gety", (Func<int>)GetY));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "getz", (Func<int>)GetZ));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "getserial", (Func<string, string>)GetSerial));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "dead", (Func<int>)Dead));

            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "findtype", (Action<string>)FindTypeSubrutine.FindType));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "findtype", (Action<int>)FindTypeSubrutine.FindType));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "findtype", (Action<int, int, int>)FindTypeSubrutine.FindType));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "findtype", (Action<string, string, string>)FindTypeSubrutine.FindType));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "findcount", (Func<int>)(() => FindTypeSubrutine.FindCount)));

            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "click", (Action<string>)Click));

            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "warmode", (Action<int>)SetWarMode));

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
        public int GetY() => api.Me.Location.Y;
        public int GetZ() => api.Me.Location.Z;
        public string GetSerial(string id) => GetObject(id);
        public int Dead() => api.Me.IsDead ? 1 : 0;

        public void Click(string id) => api.Click((uint)runtime.GetObject(id));

        public void SetWarMode(int mode)
        {
            if (mode == 0)
                api.WarModeOff();
            else
                api.WarModeOn();
        }

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

        private string GetObject(string id)
        {
            if (id.Equals("finditem", StringComparison.OrdinalIgnoreCase))
                return NumberConversions.Int2Hex(FindTypeSubrutine.FindItem);

            return NumberConversions.Int2Hex(runtime.GetObject(id));
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
