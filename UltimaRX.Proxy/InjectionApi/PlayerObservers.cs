using System;
using System.Collections.Concurrent;
using System.Threading;
using UltimaRX.Packets;
using UltimaRX.Packets.Both;
using UltimaRX.Packets.Client;
using UltimaRX.Packets.Server;

namespace UltimaRX.Proxy.InjectionApi
{
    internal class PlayerObservers
    {
        private readonly Player player;

        public PlayerObservers(Player player, ClientPacketHandler clientPacketHandler, ServerPacketHandler serverPacketHandler)
        {
            this.player = player;

            clientPacketHandler.Subscribe(PacketDefinitions.MoveRequest, HandleMoveRequest);

            serverPacketHandler.Subscribe(PacketDefinitions.CharacterLocaleAndBody, HandleCharLocaleAndBodyPacket);
            serverPacketHandler.Subscribe(PacketDefinitions.DrawGamePlayer, HandleDrawGamePlayerPacket);
            serverPacketHandler.Subscribe(PacketDefinitions.CharMoveRejection, HandleCharMoveRejectionPacket);
            serverPacketHandler.Subscribe(PacketDefinitions.CharacterMoveAck, HandleCharacterMoveAckPacket);
            serverPacketHandler.Subscribe(PacketDefinitions.UpdateCurrentHealth, HandleUpdateCurrentHealthPacket);
        }

        private void HandleUpdateCurrentHealthPacket(UpdateCurrentHealthPacket packet)
        {
            if (player.PlayerId == packet.PlayerId)
            {
                player.CurrentHealth = packet.CurrentHealth;
                player.MaxHealth = packet.MaxHealth;
            }
        }

        private void HandleCharLocaleAndBodyPacket(CharLocaleAndBodyPacket packet)
        {
            player.PlayerId = packet.PlayerId;
            player.Location = packet.Location;
            player.Movement = new Movement(packet.Direction, MovementType.Walk);
        }

        private void HandleDrawGamePlayerPacket(DrawGamePlayerPacket packet)
        {
            if (packet.PlayerId == player.PlayerId)
            {
                player.Location = packet.Location;
                player.Movement = packet.Movement;
                player.ResetWalkRequestQueue();
                OnWalkRequestDequeued();

                player.CurrentSequenceKey = 0;
                player.Color = packet.Color;
                player.BodyType = packet.BodyType;
                Program.Diagnostic.WriteLine(
                    $"WalkRequestQueue cleared, currentSequenceKey = {player.CurrentSequenceKey}");
            }
        }

        private void HandleCharMoveRejectionPacket(CharMoveRejectionPacket packet)
        {
            player.Location = packet.Location;
            player.Movement = packet.Movement;
            player.CurrentSequenceKey = 0;
            player.ResetWalkRequestQueue();
            OnWalkRequestDequeued();

            Program.Diagnostic.WriteLine(
                $"CharMoveRejection: currentSequenceKey={player.CurrentSequenceKey}, new location:{player.Location}, new direction:{player.Movement}");
        }

        public event EventHandler WalkRequestDequeued;

        private void HandleCharacterMoveAckPacket(CharacterMoveAckPacket packet)
        {
            WalkRequest walkRequest;
            if (player.WalkRequestQueue.TryDequeue(out walkRequest))
            {
                Program.Diagnostic.WriteLine($"WalkRequest dequeued, queue length: {player.WalkRequestQueue.Count}");
                OnWalkRequestDequeued();

                if (walkRequest.IssuedByProxy)
                {
                    var drawGamePlayerPacket = new DrawGamePlayerPacket(player.PlayerId, player.BodyType, player.Location, player.Movement, player.Color);
                    Program.SendToClient(drawGamePlayerPacket.RawPacket);
                }

                if (player.Movement.Direction != walkRequest.Movement.Direction)
                {
                    Program.Diagnostic.WriteLine(
                        $"Old direction: {player.Movement}, new direction: {walkRequest.Movement} - no position change");
                    player.Movement = walkRequest.Movement;
                }
                else
                {
                    Program.Diagnostic.WriteLine($"Old location: {player.Location}, direction = {walkRequest.Movement}");
                    player.Location = player.Location.LocationInDirection(walkRequest.Movement.Direction);
                    Program.Diagnostic.WriteLine($"New location: {player.Location}");
                }
            }
            else
            {
                Program.Diagnostic.WriteLine($"CharacterMoveAck received but MoveRequestQueue is empty.");
            }
        }

        private void HandleMoveRequest(MoveRequest packet)
        {
            player.CurrentSequenceKey = (byte)(packet.SequenceKey + 1);
            player.WalkRequestQueue.Enqueue(new WalkRequest(packet.SequenceKey,
                packet.Movement, false));
            Program.Diagnostic.WriteLine($"MoveRequest from client: WalkRequest enqueued, {packet.Movement}, packetSequenceKey={packet.SequenceKey}, currentSequenceKey = {player.CurrentSequenceKey}, queue length = {player.WalkRequestQueue.Count}");
        }

        private void OnWalkRequestDequeued()
        {
            WalkRequestDequeued?.Invoke(this, EventArgs.Empty);
        }
    }
}
