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
        }

        private void HandleCharLocaleAndBodyPacket(CharLocaleAndBodyPacket packet)
        {
            player.PlayerId = packet.PlayerId;
            player.Location = packet.Location;
            player.Direction = packet.Direction;
        }

        private void HandleDrawGamePlayerPacket(DrawGamePlayerPacket packet)
        {
            if (packet.PlayerId == player.PlayerId)
            {
                player.Location = packet.Location;
                player.Direction = packet.Movement.Direction;
                player.ResetWalkRequestQueue();
                player.CurrentSequenceKey = 0;
                Program.Diagnostic.WriteLine(
                    $"WalkRequestQueue cleared, currentSequenceKey = {player.CurrentSequenceKey}");
            }
        }

        private void HandleCharMoveRejectionPacket(CharMoveRejectionPacket packet)
        {
            player.Location = packet.Location;
            player.Direction = packet.Movement.Direction;
            player.CurrentSequenceKey = 0;
            player.ResetWalkRequestQueue();

            Program.Diagnostic.WriteLine(
                $"CharMoveRejection: currentSequenceKey={player.CurrentSequenceKey}, new location:{player.Location}, new direction:{player.Direction}");
        }

        private void HandleCharacterMoveAckPacket(CharacterMoveAckPacket packet)
        {
            WalkRequest walkRequest;
            if (player.WalkRequestQueue.TryDequeue(out walkRequest))
            {
                Program.Diagnostic.WriteLine($"WalkRequest dequeued, queue length: {player.WalkRequestQueue.Count}");

                if (walkRequest.IssuedByProxy)
                {
                    Program.Diagnostic.WriteLine("WalkRequest issued by proxy, sending MovePlayerPacket to client");
                    var movePlayerPacket = new MovePlayerPacket
                    {
                        Direction = walkRequest.Direction
                    };

                    Program.SendToClient(movePlayerPacket.RawPacket);
                }

                if (player.Direction != walkRequest.Direction)
                {
                    Program.Diagnostic.WriteLine(
                        $"Old direction: {player.Direction}, new direction: {walkRequest.Direction} - no position change");
                    player.Direction = walkRequest.Direction;
                }
                else
                {
                    Program.Diagnostic.WriteLine($"Old location: {player.Location}, direction = {walkRequest.Direction}");
                    player.Location = player.Location.LocationInDirection(walkRequest.Direction);
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
                packet.Movement.Direction, false));
            Program.Diagnostic.WriteLine($"MoveRequest from client: WalkRequest enqueued, {packet.Movement}, packetSequenceKey={packet.SequenceKey}, currentSequenceKey = {player.CurrentSequenceKey}, queue length = {player.WalkRequestQueue.Count}");
        }
    }
}
