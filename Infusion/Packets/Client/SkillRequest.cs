using System;
using System.Collections.Generic;
using Infusion.IO;

namespace Infusion.Packets.Client
{
    public class SkillRequest
    {
        private static readonly Dictionary<Skill, string> skills = new Dictionary<Skill, string>
        {
            {Skill.Anatomy, "1 0"},
            {Skill.AnimalLore, "2 0"},
            {Skill.ItemIdentification, "3 0"},
            {Skill.ArmsLore, "4 0"},
            {Skill.Begging, "6 0"},
            {Skill.Peacemaking, "9 0"},
            {Skill.Cartography, "12 0"},
            {Skill.DetectingHidden, "14 0"},
            {Skill.DiscordanceEnticement, "15 0"},
            {Skill.EvaluateIntelligence, "16 0"},
            {Skill.ForensicEvaluation, "19 0"},
            {Skill.Hiding, "21 0"},
            {Skill.Provocation, "22 0"},
            {Skill.Inscription, "23 0"},
            {Skill.Poisoning, "30 0"},
            {Skill.SpiritSpeak, "32 0"},
            {Skill.Stealing, "33 0"},
            {Skill.AnimalTaming, "35 0"},
            {Skill.TasteIdentification, "36 0"},
            {Skill.Tracking, "38 0"},
            {Skill.Meditation, "46 0"}
        };

        private static readonly Dictionary<Spell, string> spells = new Dictionary<Spell, string>
        {
            {Spell.CreateFood, "2"},
            {Spell.Feeblemind, "3"},
            {Spell.Heal, "4"},
            {Spell.MagicArrow, "5"},
            {Spell.NightSight, "6"},
            {Spell.ReactiveArmor, "7"},
            {Spell.Weaken, "8"},
            {Spell.Agility, "9"},
            {Spell.Cunning, "10"},
            {Spell.Cure, "11"},
            {Spell.Harm, "12"},
            {Spell.MagicTrap, "13"},
            {Spell.MagicUntrap, "14"},
            {Spell.Protection, "15"},
            {Spell.Strength, "16"},
            {Spell.Bless, "17"},
            {Spell.Fireball, "18"},
            {Spell.MagicLock, "19"},
            {Spell.Poison, "20"},
            {Spell.Telekenisis, "21"},
            {Spell.Teleport, "22"},
            {Spell.Unlock, "23"},
            {Spell.WallOfStone, "24"},
            {Spell.ArchCure, "25"},
            {Spell.ArchProtection, "26"},
            {Spell.Curse, "27"},
            {Spell.FireField, "28"},
            {Spell.GreaterHeal, "29"},
            {Spell.Lightning, "30"},
            {Spell.ManaDrain, "31"},
            {Spell.Recall, "32"},
            {Spell.BladeSpirit, "33"},
            {Spell.DispelField, "34"},
            {Spell.Incognito, "35"},
            {Spell.Reflection, "36"},
            {Spell.MindBlast, "37"},
            {Spell.Paralyze, "38"},
            {Spell.PoisonField, "39"},
            {Spell.SummonCreature, "40"},
            {Spell.Dispel, "41"},
            {Spell.EnergyBolt, "42"},
            {Spell.Explosion, "43"},
            {Spell.Invisibility, "44"},
            {Spell.Mark, "45"},
            {Spell.MassCurse, "46"},
            {Spell.ParalyzeField, "47"},
            {Spell.Reveal, "48"},
            {Spell.ChainLightning, "49"},
            {Spell.EnergyField, "50"},
            {Spell.FlameStrike, "51"},
            {Spell.Gate, "52"},
            {Spell.ManaVampire, "53"},
            {Spell.MassDispel, "54"},
            {Spell.MeteorShower, "55"},
            {Spell.Polymorph, "56"},
            {Spell.Earthquake, "57"},
            {Spell.EnergyVortex, "58"},
            {Spell.Ressurection, "59"},
            {Spell.SummonAirElemental, "60"},
            {Spell.SummonDaemon, "61"},
            {Spell.SummonEarthElemental, "62"},
            {Spell.SummonFireElemental, "63"},
            {Spell.SummonWaterElemental, "64"}
        };

        public SkillRequest(byte type, string action)
        {
            var packetLength = (ushort) 5;
            var payload = new byte[packetLength];

            var writer = new ArrayPacketWriter(payload);
            writer.WriteByte((byte)PacketDefinitions.RequestSkills.Id);
            writer.WriteUShort(packetLength);
            writer.WriteByte(type);
            if (string.IsNullOrEmpty(action))
                writer.WriteByte(0);
            else
                writer.WriteString(action);

            RawPacket = new Packet(PacketDefinitions.RequestSkills.Id, payload);
        }

        public SkillRequest(Skill skill)
        {
            if (!skills.TryGetValue(skill, out string skillString))
                throw new InvalidOperationException($"Cannot request {skill}");

            var packetLength = (ushort) (5 + skillString.Length);
            var payload = new byte[packetLength];

            var writer = new ArrayPacketWriter(payload);
            writer.WriteByte((byte) PacketDefinitions.RequestSkills.Id);
            writer.WriteUShort(packetLength);
            writer.WriteByte(0x24);
            writer.WriteString(skillString);
            writer.WriteByte(0x00);

            RawPacket = new Packet(PacketDefinitions.RequestSkills.Id, payload);
        }

        public SkillRequest(Spell spell)
        {
            var spellString = spells[spell];

            var packetLength = (ushort) (5 + spellString.Length);
            var payload = new byte[packetLength];

            var writer = new ArrayPacketWriter(payload);
            writer.WriteByte((byte) PacketDefinitions.RequestSkills.Id);
            writer.WriteUShort(packetLength);
            writer.WriteByte(0x56);
            writer.WriteString(spellString);
            writer.WriteByte(0x00);

            RawPacket = new Packet(PacketDefinitions.RequestSkills.Id, payload);
        }

        public Packet RawPacket { get; }
    }
}