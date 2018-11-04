using Infusion.LegacyApi.Console;
using InjectionScript.Interpretation;
using InjectionScript.Parsing;
using System;

namespace Infusion.LegacyApi.Injection
{
    public sealed class InjectionHost
    {
        private Runtime runtime;
        private readonly Legacy api;
        private readonly IConsole console;
        private readonly SpeechJournal journal;

        public InjectionHost(Legacy api, IConsole console)
        {
            this.api = api;
            this.console = console;
            journal = api.CreateSpeechJournal();
        }

        public void LoadScript(string fileName)
        {
            runtime = new Runtime();

            try
            {
                runtime.Load(fileName);

                RegisterNatives();
            }
            catch (SyntaxErrorException ex)
            {
                foreach (var error in ex.Errors)
                    console.Error($"{error.Line}, {error.CharPos} {error.Message}");
            }
        }

        public void CallSubrutine(string subrutineName)
        {
            runtime.CallSubrutine(subrutineName);
        }

        private void RegisterNatives()
        {
            runtime.Metadata.Add(new NativeSubrutineDefinition("wait", (Action<int>)Wait));

            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "print", (Action<string>)Print));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "getx", (Func<int>)GetX));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "gety", (Func<int>)GetY));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "getz", (Func<int>)GetZ));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "dead", (Func<int>)Dead));

            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "deletejournal", (Action)DeleteJournal));

            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "warmode", (Action<int>)SetWarMode));

            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "useskill", (Action<string>)UseSkill));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "cast", (Action<string>)Cast));
        }

        private void Wait(int ms) => api.Wait(ms);
        private void Print(string msg) => api.ClientPrint(msg);
        private int GetX() => api.Me.Location.X;
        private int GetY() => api.Me.Location.Y;
        private int GetZ() => api.Me.Location.Z;
        private int Dead() => api.Me.IsDead ? 1 : 0;

        private void DeleteJournal() => journal.Delete();

        private void SetWarMode(int mode)
        {
            if (mode == 0)
                api.WarModeOff();
            else
                api.WarModeOn();
        }

        private void UseSkill(string skillName) => api.UseSkill(TranslateSkill(skillName));
        private void Cast(string spellName) => api.CastSpell(TranslateSpell(spellName));

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
