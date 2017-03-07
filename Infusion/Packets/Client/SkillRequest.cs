using System.Collections.Generic;
using Infusion.IO;

namespace Infusion.Packets.Client
{
    public class SkillRequest
    {
        private static readonly Dictionary<RequestableSkill, string> Skills = new Dictionary<RequestableSkill, string>
        {
            {RequestableSkill.Anatomy, "1 0"},
            {RequestableSkill.AnimalLore, "2 0"},
            {RequestableSkill.ItemIdentification, "3 0"},
            {RequestableSkill.ArmsLore, "4 0"},
            {RequestableSkill.Begging, "6 0"},
            {RequestableSkill.Peacemaking, "9 0"},
            {RequestableSkill.Cartography, "12 0"},
            {RequestableSkill.DetectingHidden, "14 0"},
            {RequestableSkill.DiscordanceEnticement, "15 0"},
            {RequestableSkill.EvaluateIntelligence, "16 0"},
            {RequestableSkill.ForensicEvaluation, "19 0"},
            {RequestableSkill.Hiding, "21 0"},
            {RequestableSkill.Provocation, "22 0"},
            {RequestableSkill.Inscription, "23 0"},
            {RequestableSkill.Poisoning, "30 0"},
            {RequestableSkill.SpiritSpeak, "32 0"},
            {RequestableSkill.Stealing, "33 0"},
            {RequestableSkill.AnimalTaming, "35 0"},
            {RequestableSkill.TasteIdentification, "36 0"},
            {RequestableSkill.Tracking, "38 0"}
        };

        private static readonly Dictionary<RequestableSpell, string> Spells = new Dictionary<RequestableSpell, string>
        {
            {RequestableSpell.CreateFood, "2"},
            {RequestableSpell.Feeblemind, "3"},
            {RequestableSpell.Heal, "4"},
            {RequestableSpell.MagicArrow, "5"},
            {RequestableSpell.NightSight, "6"},
            {RequestableSpell.ReactiveArmor, "7"},
            {RequestableSpell.Weaken, "8"},
            {RequestableSpell.Agility, "9"},
            {RequestableSpell.Cunning, "10"},
            {RequestableSpell.Cure, "11"},
            {RequestableSpell.Harm, "12"},
            {RequestableSpell.MagicTrap, "13"},
            {RequestableSpell.MagicUntrap, "14"},
            {RequestableSpell.Protection, "15"},
            {RequestableSpell.Strength, "16"},
            {RequestableSpell.Bless, "17"},
            {RequestableSpell.Fireball, "18"},
            {RequestableSpell.MagicLock, "19"},
            {RequestableSpell.Poison, "20"},
            {RequestableSpell.Telekenisis, "21"},
            {RequestableSpell.Teleport, "22"},
            {RequestableSpell.Unlock, "23"},
            {RequestableSpell.WallOfStone, "24"},
            {RequestableSpell.ArchCure, "25"},
            {RequestableSpell.ArchProtection, "26"},
            {RequestableSpell.Curse, "27"},
            {RequestableSpell.FireField, "28"},
            {RequestableSpell.GreaterHeal, "29"},
            {RequestableSpell.Lightning, "30"},
            {RequestableSpell.ManaDrain, "31"},
            {RequestableSpell.Recall, "32"},
            {RequestableSpell.BladeSpirit, "33"},
            {RequestableSpell.DispelField, "34"},
            {RequestableSpell.Incognito, "35"},
            {RequestableSpell.Reflection, "36"},
            {RequestableSpell.MindBlast, "37"},
            {RequestableSpell.Paralyze, "38"},
            {RequestableSpell.PoisonField, "39"},
            {RequestableSpell.SummonCreature, "40"},
            {RequestableSpell.Dispel, "41"},
            {RequestableSpell.EnergyBolt, "42"},
            {RequestableSpell.Explosion, "43"},
            {RequestableSpell.Invisibility, "44"},
            {RequestableSpell.Mark, "45"},
            {RequestableSpell.MassCurse, "46"},
            {RequestableSpell.ParalyzeField, "47"},
            {RequestableSpell.Reveal, "48"},
            {RequestableSpell.ChainLightning, "49"},
            {RequestableSpell.EnergyField, "50"},
            {RequestableSpell.FlameStrike, "51"},
            {RequestableSpell.Gate, "52"},
            {RequestableSpell.ManaVampire, "53"},
            {RequestableSpell.MassDispel, "54"},
            {RequestableSpell.MeteorShower, "55"},
            {RequestableSpell.Polymorph, "56"},
            {RequestableSpell.Earthquake, "57"},
            {RequestableSpell.EnergyVortex, "58"},
            {RequestableSpell.Ressurection, "59"},
            {RequestableSpell.SummonAirElemental, "60"},
            {RequestableSpell.SummonDaemon, "61"},
            {RequestableSpell.SummonEarthElemental, "62"},
            {RequestableSpell.SummonFireElemental, "63"},
            {RequestableSpell.SummonWaterElemental, "64"}
        };

        public SkillRequest(RequestableSkill skill)
        {
            var skillString = Skills[skill];

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

        public SkillRequest(RequestableSpell spell)
        {
            var spellString = Spells[spell];

            var packetLength = (ushort)(5 + spellString.Length);
            var payload = new byte[packetLength];

            var writer = new ArrayPacketWriter(payload);
            writer.WriteByte((byte)PacketDefinitions.RequestSkills.Id);
            writer.WriteUShort(packetLength);
            writer.WriteByte(0x56);
            writer.WriteString(spellString);
            writer.WriteByte(0x00);

            RawPacket = new Packet(PacketDefinitions.RequestSkills.Id, payload);
        }

        public Packet RawPacket { get; }
    }
}