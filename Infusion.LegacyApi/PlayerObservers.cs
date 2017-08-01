using System;
using System.Threading;
using Infusion.Logging;
using Infusion.Packets;
using Infusion.Packets.Both;
using Infusion.Packets.Client;
using Infusion.Packets.Server;

namespace Infusion.LegacyApi
{
    internal class PlayerObservers
    {
        private readonly UltimaClient client;
        private readonly UltimaServer server;
        private readonly Player player;
        private readonly ILogger logger;
        private readonly Legacy legacyApi;
        private bool discardNextClientAck;

        public PlayerObservers(Player player, UltimaClient client, UltimaServer server, ILogger logger, Legacy legacyApi)
        {
            this.client = client;
            this.server = server;
            this.player = player;
            this.logger = logger;
            this.legacyApi = legacyApi;

            client.RegisterFilter(FilterClientPackets);
            client.Subscribe(PacketDefinitions.MoveRequest, HandleMoveRequest);

            server.RegisterFilter(FilterServerPackets);
            server.Subscribe(PacketDefinitions.CharacterLocaleAndBody, HandleCharLocaleAndBodyPacket);
            server.Subscribe(PacketDefinitions.DrawGamePlayer, HandleDrawGamePlayerPacket);
            server.Subscribe(PacketDefinitions.CharMoveRejection, HandleCharMoveRejectionPacket);
            server.Subscribe(PacketDefinitions.UpdateCurrentHealth, HandleUpdateCurrentHealthPacket);
            server.Subscribe(PacketDefinitions.UpdateCurrentStamina, HandleUpdateCurrentStaminaPacket);
            server.Subscribe(PacketDefinitions.UpdateCurrentMana, HandleUpdateCurrentManaPacket);
            server.Subscribe(PacketDefinitions.StatusBarInfo, HandleStatusBarInfoPacket);
            server.Subscribe(PacketDefinitions.SendSkills, HandleSendSkillsPacket);
            server.Subscribe(PacketDefinitions.AllowRefuseAttack, HandleAllowRefuseAttack);
        }

        private ObjectId acceptedAttackTargedId;
        private readonly AutoResetEvent attackResponseReceivedEvent = new AutoResetEvent(false);

        public AttackResult Attack(ObjectId targetId, TimeSpan? timeout = null)
        {
            acceptedAttackTargedId = new ObjectId(0);

            server.AttackRequest(targetId);

            var totalMilliseconds = 0;
            while (!attackResponseReceivedEvent.WaitOne(100))
            {
                totalMilliseconds += 100;
                if (timeout.HasValue && totalMilliseconds > timeout.Value.TotalMilliseconds)
                    return AttackResult.Timeout;

                legacyApi.CheckCancellation();
            }

            if (acceptedAttackTargedId.Value == 0)
                return AttackResult.Refused;

            return AttackResult.Accepted;
        }

        private void HandleAllowRefuseAttack(AllowRefuseAttackPacket packet)
        {
            acceptedAttackTargedId = packet.AttackTargetId;
            attackResponseReceivedEvent.Set();
        }

        private void HandleSendSkillsPacket(SendSkillsPacket packet)
        {
            if (packet.Values.Length == 1)
            {
                SkillValue currentSkillValue;
                if (player.Skills.TryGetValue(packet.Values[0].Skill, out currentSkillValue))
                {
                    if (currentSkillValue.Value < packet.Values[0].Value)
                    {
                        var delta = packet.Values[0].Percentage - currentSkillValue.Percentage;
                        logger.Info(
                            $"Skill {packet.Values[0].Skill} increased by {delta:F1} %, currently it is {packet.Values[0].Percentage:F1} %");
                    }
                }
            }

            player.UpdateSkills(packet.Values);
        }

        private void HandleStatusBarInfoPacket(StatusBarInfoPacket packet)
        {
            if (packet.PlayerId == player.PlayerId)
            {
                player.CurrentStamina = packet.CurrentStamina;
                player.MaxStamina = packet.MaxStamina;
                player.CurrentHealth = packet.CurrentHealth;
                player.MaxHealth = packet.MaxHealth;
                player.CurrentMana = packet.CurrentMana;
                player.MaxMana = packet.MaxMana;
                player.Weight = packet.Weight;
                player.Intelligence = packet.Intelligence;
                player.Dexterity = packet.Dexterity;
                player.Strength = packet.Strength;
            }
        }

        private Packet? FilterClientPackets(Packet rawPacket)
        {
            if (discardNextClientAck && rawPacket.Id == PacketDefinitions.CharacterMoveAck.Id)
            {
                var packet = PacketDefinitionRegistry.Materialize<CharacterMoveAckPacket>(rawPacket);

                if (packet.MovementSequenceKey == 0)
                {
                    discardNextClientAck = false;
                    return null;
                }
            }

            return rawPacket;
        }

        private void HandleUpdateCurrentHealthPacket(UpdateCurrentHealthPacket packet)
        {
            if (player.PlayerId == packet.PlayerId)
            {
                player.CurrentHealth = packet.CurrentHealth;
                player.MaxHealth = packet.MaxHealth;
            }
        }

        private void HandleUpdateCurrentStaminaPacket(UpdateCurrentStaminaPacket packet)
        {
            if (player.PlayerId == packet.PlayerId)
            {
                player.CurrentStamina = packet.CurrentStamina;
                player.MaxStamina = packet.MaxStamina;
            }
        }

        private void HandleUpdateCurrentManaPacket(UpdateCurrentManaPacket packet)
        {
            if (player.PlayerId == packet.PlayerId)
            {
                player.CurrentMana = packet.CurrentMana;
                player.MaxMana = packet.MaxMana;
            }
        }

        private void HandleCharLocaleAndBodyPacket(CharLocaleAndBodyPacket packet)
        {
            player.PlayerId = packet.PlayerId;
            player.Location = packet.Location;
            player.PredictedLocation = packet.Location;
            player.Movement = packet.Movement;
            player.PredictedMovement = player.Movement;
        }

        private void HandleDrawGamePlayerPacket(DrawGamePlayerPacket packet)
        {
            if (packet.PlayerId == player.PlayerId)
            {
                player.Location = packet.Location;
                player.PredictedLocation = packet.Location;
                player.Movement = packet.Movement;
                player.PredictedMovement = player.Movement;
                player.ResetWalkRequestQueue();

                player.CurrentSequenceKey = 0;
                player.Color = packet.Color;
                player.BodyType = packet.BodyType;

                OnWalkRequestDequeued();
            }
        }

        private void HandleCharMoveRejectionPacket(CharMoveRejectionPacket packet)
        {
            player.Location = packet.Location;
            player.PredictedLocation = packet.Location;
            player.Movement = packet.Movement;
            player.PredictedMovement = player.Movement;
            player.CurrentSequenceKey = 0;
            player.ResetWalkRequestQueue();

            OnWalkRequestDequeued();
        }

        public event EventHandler WalkRequestDequeued;

        private Packet? FilterServerPackets(Packet rawPacket)
        {
            WalkRequest walkRequest;
            var discardCurrentPacket = false;

            if (rawPacket.Id != PacketDefinitions.CharacterMoveAck.Id)
            {
                return rawPacket;
            }

            if (player.WalkRequestQueue.TryDequeue(out walkRequest))
            {
                try
                {
                    if (walkRequest.IssuedByProxy)
                    {
                        discardNextClientAck = true;

                        client.DrawGamePlayer(player.PlayerId, player.BodyType,
                            player.Location, player.Movement, player.Color);

                        discardCurrentPacket = true;
                    }

                    if (player.Movement.Direction != walkRequest.Movement.Direction)
                        player.Movement = walkRequest.Movement;
                    else
                        player.Location = player.Location.LocationInDirection(walkRequest.Movement.Direction);

                }
                finally
                {
                    OnWalkRequestDequeued();
                }
            }


            return discardCurrentPacket ? (Packet?) null : rawPacket;
        }

        private void HandleMoveRequest(MoveRequest packet)
        {
            player.CurrentSequenceKey = (byte) (packet.SequenceKey + 1);
            player.WalkRequestQueue.Enqueue(new WalkRequest(packet.SequenceKey,
                packet.Movement, false));
        }

        private void OnWalkRequestDequeued()
        {
            WalkRequestDequeued?.Invoke(this, EventArgs.Empty);
        }
    }
}