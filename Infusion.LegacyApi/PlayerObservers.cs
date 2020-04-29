using Infusion.LegacyApi.Console;
using Infusion.LegacyApi.Events;
using Infusion.Logging;
using Infusion.Packets;
using Infusion.Packets.Both;
using Infusion.Packets.Client;
using Infusion.Packets.Server;
using System;
using System.Threading;

namespace Infusion.LegacyApi
{
    internal sealed class PlayerObservers
    {
        private readonly UltimaClient client;
        private readonly UltimaServer server;
        private readonly Player player;
        private readonly IConsole console;
        private readonly Legacy legacyApi;
        private readonly GameObjectCollection gameObjects;
        private readonly EventJournalSource eventJournalSource;
        private readonly PacketDefinitionRegistry packetRegistry;

        public event Action LoginConfirmed;

        public PlayerObservers(Player player, UltimaClient client, UltimaServer server, IConsole console,
            Legacy legacyApi, GameObjectCollection gameObjects, EventJournalSource eventJournalSource,
            PacketDefinitionRegistry packetRegistry)
        {
            this.client = client;
            this.server = server;
            this.player = player;
            this.console = console;
            this.legacyApi = legacyApi;
            this.gameObjects = gameObjects;
            this.eventJournalSource = eventJournalSource;
            this.packetRegistry = packetRegistry;
            IClientPacketSubject clientPacketSubject = client;
            clientPacketSubject.Subscribe(PacketDefinitions.MoveRequest, HandleMoveRequest);
            clientPacketSubject.RegisterOutputFilter(FilterSentClientPackets);

            server.RegisterFilter(FilterServerPackets);
            
            server.Subscribe(PacketDefinitions.CharacterLocaleAndBody, HandleCharLocaleAndBodyPacket);
            server.Subscribe(PacketDefinitions.DrawGamePlayer, HandleDrawGamePlayerPacket);
            server.Subscribe(PacketDefinitions.CharMoveRejection, HandleCharMoveRejectionPacket);
            server.Subscribe(PacketDefinitions.UpdateCurrentHealth, HandleUpdateCurrentHealthPacket);
            server.Subscribe(PacketDefinitions.UpdateCurrentStamina, HandleUpdateCurrentStaminaPacket);
            server.Subscribe(PacketDefinitions.UpdateCurrentMana, HandleUpdateCurrentManaPacket);
            server.Subscribe(PacketDefinitions.StatusBarInfo, HandleStatusBarInfoPacket);
            server.Subscribe(PacketDefinitions.SendSkills, HandleSendSkillsPacket);
            server.Subscribe(PacketDefinitions.DrawObject, HandleDrawObjectPacket);
            server.Subscribe(PacketDefinitions.AllowRefuseAttack, HandleAllowRefuseAttack);
            server.Subscribe(PacketDefinitions.UpdatePlayer, HandleUpdatePlayerPacket);

            clientPacketSubject.Subscribe(PacketDefinitions.RequestSkills, HandleRequestSkills);
        }

        private Skill? lastSkill;

        private Packet? FilterSentClientPackets(Packet rawPacket)
        {
            if (rawPacket.Id == PacketDefinitions.RequestSkills.Id)
            {
                var packet = packetRegistry.Materialize<SkillRequest>(rawPacket);
                lastSkill = packet.Skill;
            }

            return rawPacket;
        }

        private void HandleUpdatePlayerPacket(UpdatePlayerPacket packet)
        {
            if (packet.PlayerId == player.PlayerId)
            {
                player.Flags = packet.Flags;
                player.BodyType = packet.Type;
                player.Direction = packet.Direction;
                player.Location = packet.Location;
                player.MovementType = packet.MovementType;
                player.Color = packet.Color;
            }
        }

        private void HandleRequestSkills(SkillRequest packet)
        {
            if (packet.Skill.HasValue)
                eventJournalSource.Publish(new SkillRequestedEvent(packet.Skill.Value));
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
                var skill = packet.Values[0].Skill != Skill.None ? packet.Values[0].Skill : lastSkill ?? Skill.None;
                if (skill == Skill.None)
                {
                    throw new NotImplementedException("Server sent 3a packet with skill 0 and no skill was requested from client.");
                }

                if (player.Skills.TryGetValue(skill, out var currentSkillValue))
                {
                    if (currentSkillValue.Value != packet.Values[0].Value)
                    {
                        eventJournalSource.Publish(new SkillChangedEvent(skill, currentSkillValue.Value, packet.Values[0].Value));
                    }

                    if (currentSkillValue.Value < packet.Values[0].Value)
                    {
                        var delta = packet.Values[0].Percentage - currentSkillValue.Percentage;
                        console.WriteLine(ConsoleLineType.SkillChanged, 
                            $"Skill {skill} increased by {delta:F1} %, currently it is {packet.Values[0].Percentage:F1} %");
                    }
                }
            }

            player.UpdateSkills(packet.Values);
        }

        private void HandleStatusBarInfoPacket(StatusBarInfoPacket packet)
        {
            if (packet.PlayerId == player.PlayerId)
            {
                player.Name = packet.PlayerName;
                player.Armor = packet.Armor;
                player.CurrentStamina = packet.CurrentStamina;
                player.MaxStamina = packet.MaxStamina;
                player.CurrentHealth = packet.CurrentHealth;
                player.MaxHealth = packet.MaxHealth;
                player.CurrentMana = packet.CurrentMana;
                player.MaxMana = packet.MaxMana;
                player.Weight = packet.Weight;
                player.Gold = packet.Gold;
                player.Intelligence = packet.Intelligence;
                player.Dexterity = packet.Dexterity;
                player.Strength = packet.Strength;

                // spaghetti hack: gameObjects health is handled in ItemsObservers
            }
        }

        private void HandleUpdateCurrentHealthPacket(UpdateCurrentHealthPacket packet)
        {
            if (player.PlayerId == packet.PlayerId)
            {
                player.CurrentHealth = packet.CurrentHealth;
                player.MaxHealth = packet.MaxHealth;

                // spaghetti hack: gameObjects health is handled in ItemsObservers
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
            player.Direction = packet.Direction;
            player.MovementType = packet.MovementType;
            player.PredictedDirection = player.Direction;

            eventJournalSource.Publish(new LoginConfirmedEvent());
            LoginConfirmed?.Invoke();
        }

        private void HandleDrawGamePlayerPacket(DrawGamePlayerPacket packet)
        {
            if (packet.PlayerId == player.PlayerId)
            {
                player.Location = packet.Location;
                player.PredictedLocation = packet.Location;
                player.Direction = packet.Direction;
                player.MovementType = packet.MovementType;
                player.PredictedDirection = player.Direction;
                player.ResetWalkRequestQueue();
                player.Flags = packet.Flags;

                player.CurrentSequenceKey = 0;
                player.Color = packet.Color;
                player.BodyType = packet.BodyType;

                if (gameObjects[packet.PlayerId] is Mobile mobile)
                {
                    gameObjects.UpdateObject(mobile.Update(packet.BodyType, packet.Location, packet.Color, packet.Direction,
                        packet.MovementType, Notoriety.Friend, packet.Flags));
                }
                else
                {
                    mobile = new Mobile(packet.PlayerId, packet.BodyType, packet.Location, packet.Color,
                        packet.Direction, packet.MovementType,
                        Notoriety.Friend, packet.Flags);
                    gameObjects.AddObject(mobile);
                }

                OnWalkRequestDequeued();
            }
        }

        private void HandleDrawObjectPacket(DrawObjectPacket packet)
        {
            if (packet.Id == legacyApi.Me.PlayerId)
            {
                player.Location = packet.Location;
                player.PredictedLocation = packet.Location;
                player.Direction = packet.Direction;
                player.PredictedDirection = packet.Direction;
                player.MovementType = packet.MovementType;

                player.Color = packet.Color;
                player.BodyType = packet.Type;
                player.Flags = packet.Flags;
                player.Notoriety = packet.Notoriety;

                player.ResetWalkRequestQueue();
                player.CurrentSequenceKey = 0;
                OnWalkRequestDequeued();

                if (gameObjects[packet.Id] is Mobile mobile)
                {
                    gameObjects.UpdateObject(mobile.Update(packet.Type, packet.Location, packet.Color, packet.Direction,
                        packet.MovementType,
                        packet.Notoriety, packet.Flags));
                }
                else
                {
                    mobile = new Mobile(packet.Id, packet.Type, packet.Location, packet.Color,
                        packet.Direction, packet.MovementType,
                        packet.Notoriety, packet.Flags);
                    gameObjects.AddObject(mobile);
                }
            }
        }


        private void HandleCharMoveRejectionPacket(CharMoveRejectionPacket packet)
        {
            player.RejectWalkRequest(packet);

            eventJournalSource.Publish(new PlayerMoveRejectedEvent());

        }

        internal event EventHandler WalkRequestDequeued;

        private Packet? FilterServerPackets(Packet rawPacket)
        {
            var discardCurrentPacket = false;
            
            if (rawPacket.Id != PacketDefinitions.CharacterMoveAck.Id)
            {
                return rawPacket;
            }

            if (player.WalkRequestQueue.TryDequeue(out var walkRequest))
            {
                try
                {
                    if (walkRequest.IssuedByProxy)
                    {
                        client.PauseClient(PauseClientChoice.Pause);
                        client.DrawGamePlayer(player.PlayerId, player.BodyType,
                            player.Location, player.Direction, player.MovementType, player.Color);
                        foreach (var mobile in UO.Mobiles)
                        {
                            if (mobile.Id != player.PlayerId)
                            {
                                UO.Client.ObjectInfo(mobile.Id, mobile.Type, mobile.Location, mobile.Color);
                            }
                        }
                        client.PauseClient(PauseClientChoice.Resume);

                        discardCurrentPacket = true;
                    }

                    if (player.Direction != walkRequest.Direction)
                        player.Direction = walkRequest.Direction;
                    else
                        player.Location = player.Location.LocationInDirection(walkRequest.Direction);

                    if (gameObjects[player.PlayerId] is Mobile updatedMobile)
                        gameObjects.UpdateObject(updatedMobile.UpdateLocation(player.Location, player.Direction, player.MovementType));
                }
                finally
                {
                    OnWalkRequestDequeued();
                }
            }

            eventJournalSource.Publish(new PlayerMoveAcceptedEvent());

            return discardCurrentPacket ? (Packet?)null : rawPacket;
        }

        private void HandleMoveRequest(MoveRequest packet)
        {
            player.CurrentSequenceKey = (byte)(packet.SequenceKey + 1);
            player.WalkRequestQueue.Enqueue(new WalkRequest(packet.SequenceKey,
                packet.Direction, packet.MovementType, false));

            eventJournalSource.Publish(new PlayerMoveRequestedEvent(packet.Direction));
        }

        private void OnWalkRequestDequeued()
        {
            WalkRequestDequeued?.Invoke(this, EventArgs.Empty);
        }
    }
}