using System;
using Infusion.Packets;
using Infusion.Packets.Both;
using Infusion.Packets.Client;
using Infusion.Packets.Server;

namespace Infusion.Proxy.InjectionApi
{
    internal class PlayerObservers
    {
        private readonly Player player;
        private bool discardNextClientAck;

        public PlayerObservers(Player player, ClientPacketHandler clientPacketHandler,
            ServerPacketHandler serverPacketHandler)
        {
            this.player = player;

            clientPacketHandler.RegisterFilter(FilterClientPackets);
            clientPacketHandler.Subscribe(PacketDefinitions.MoveRequest, HandleMoveRequest);

            serverPacketHandler.RegisterFilter(FilterServerPackets);
            serverPacketHandler.Subscribe(PacketDefinitions.CharacterLocaleAndBody, HandleCharLocaleAndBodyPacket);
            serverPacketHandler.Subscribe(PacketDefinitions.DrawGamePlayer, HandleDrawGamePlayerPacket);
            serverPacketHandler.Subscribe(PacketDefinitions.CharMoveRejection, HandleCharMoveRejectionPacket);
            serverPacketHandler.Subscribe(PacketDefinitions.UpdateCurrentHealth, HandleUpdateCurrentHealthPacket);
            serverPacketHandler.Subscribe(PacketDefinitions.UpdateCurrentStamina, HandleUpdateCurrentStaminaPacket);
            serverPacketHandler.Subscribe(PacketDefinitions.StatusBarInfo, HandleStatusBarInfoPacket);
            serverPacketHandler.Subscribe(PacketDefinitions.SendSkills, HandleSendSkillsPacket);
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
                        Program.Console.Info(
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
                Program.Diagnostic.Debug("Received client CharacterMoveAck that is candidate for discard");
                var packet = PacketDefinitionRegistry.Materialize<CharacterMoveAckPacket>(rawPacket);

                if (packet.MovementSequenceKey == 0)
                {
                    Program.Diagnostic.Debug("Discarding client CharacterMoveAck");
                    discardNextClientAck = false;
                    return null;
                }
                Program.Diagnostic.Debug("Client CharacterMoveAck not discarded");
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

                Program.Diagnostic.Debug(
                    $"WalkRequestQueue cleared, currentSequenceKey = {player.CurrentSequenceKey}");
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

            Program.Diagnostic.Debug(
                $"CharMoveRejection: currentSequenceKey={player.CurrentSequenceKey}, new location:{player.Location}, new direction:{player.Movement}");
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
                    Program.Diagnostic.Debug($"WalkRequest dequeued, queue length: {player.WalkRequestQueue.Count}");

                    if (walkRequest.IssuedByProxy)
                    {
                        var drawGamePlayerPacket = new DrawGamePlayerPacket(player.PlayerId, player.BodyType,
                            player.Location, player.Movement, player.Color);
                        discardNextClientAck = true;
                        Program.SendToClient(drawGamePlayerPacket.RawPacket);
                        discardCurrentPacket = true;
                        Program.Diagnostic.Debug("WalkRequest issued by proxy, discarding packet");
                    }
                    else
                    {
                        Program.Diagnostic.Debug("WalkRequest issued by client, sending to client");
                    }

                    if (player.Movement.Direction != walkRequest.Movement.Direction)
                    {
                        Program.Diagnostic.Debug(
                            $"Old direction: {player.Movement}, new direction: {walkRequest.Movement} - no position change");
                        player.Movement = walkRequest.Movement;
                    }
                    else
                    {
                        Program.Diagnostic.Debug($"Old location: {player.Location}, direction = {walkRequest.Movement}");
                        player.Location = player.Location.LocationInDirection(walkRequest.Movement.Direction);
                        Program.Diagnostic.Debug($"New location: {player.Location}");
                    }

                }
                finally
                {
                    OnWalkRequestDequeued();
                }
            }
            else
            {
                Program.Diagnostic.Debug("CharacterMoveAck received but MoveRequestQueue is empty.");
            }


            return discardCurrentPacket ? (Packet?) null : rawPacket;
        }

        private void HandleMoveRequest(MoveRequest packet)
        {
            player.CurrentSequenceKey = (byte) (packet.SequenceKey + 1);
            player.WalkRequestQueue.Enqueue(new WalkRequest(packet.SequenceKey,
                packet.Movement, false));
            Program.Diagnostic.Debug(
                $"MoveRequest from client: WalkRequest enqueued, {packet.Movement}, packetSequenceKey={packet.SequenceKey}, currentSequenceKey = {player.CurrentSequenceKey}, queue length = {player.WalkRequestQueue.Count}");
        }

        private void OnWalkRequestDequeued()
        {
            WalkRequestDequeued?.Invoke(this, EventArgs.Empty);
        }
    }
}