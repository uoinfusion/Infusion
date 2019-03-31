using Infusion.LegacyApi.Events;
using Infusion.Packets;
using Infusion.Packets.Client;
using Infusion.Packets.Server;
using System;
using System.Threading;

namespace Infusion.LegacyApi
{
    internal partial class ItemsObservers
    {
        private readonly GameObjectCollection gameObjects;
        private readonly Legacy legacyApi;
        private readonly EventJournalSource eventJournalSource;
        private readonly EventJournal eventJournal;
        private readonly EventJournal waitForItemDraggedJournal;

        public AutoResetEvent WaitForItemDraggedStartedEvent => waitForItemDraggedJournal.AwaitingStarted;
        private ObjectId? itemIdDraggedByScript;

        public ItemsObservers(GameObjectCollection gameObjects, IServerPacketSubject serverPacketSubject, IClientPacketSubject clientPacketSubject,
            Legacy legacyApi, EventJournalSource eventJournalSource)
        {
            this.eventJournal = new EventJournal(eventJournalSource);
            this.waitForItemDraggedJournal = new EventJournal(eventJournalSource);

            this.gameObjects = gameObjects;
            this.gameObjects.MobileLeftView += (sender, mobile) =>
            {
                eventJournalSource.Publish(new MobileLeftViewEvent(mobile));
            };
            this.gameObjects.MobileDeleted += (sender, mobile) =>
            {
                eventJournalSource.Publish(new MobileLeftViewEvent(mobile));
            };

            this.legacyApi = legacyApi;
            this.eventJournalSource = eventJournalSource;
            serverPacketSubject.Subscribe(PacketDefinitions.AddMultipleItemsInContainer,
                HandleAddMultipleItemsInContainer);
            serverPacketSubject.Subscribe(PacketDefinitions.AddItemToContainer, HandleAddItemToContainer);
            serverPacketSubject.Subscribe(PacketDefinitions.DeleteObject, HandleDeleteObjectPacket);
            serverPacketSubject.Subscribe(PacketDefinitions.ObjectInfo, HandleObjectInfoPacket);
            serverPacketSubject.Subscribe(PacketDefinitions.SecondAgeObjectInformation7090, HandleSecondAgeObjectInfoPacket);
            serverPacketSubject.Subscribe(PacketDefinitions.DrawObject, HandleDrawObjectPacket);
            serverPacketSubject.Subscribe(PacketDefinitions.UpdatePlayer, HandleUpdatePlayerPacket);
            serverPacketSubject.Subscribe(PacketDefinitions.UpdateCurrentHealth, HandleUpdateCurrentHealthPacket);
            serverPacketSubject.Subscribe(PacketDefinitions.WornItem, HandleWornItemPacket);
            serverPacketSubject.Subscribe(PacketDefinitions.RejectMoveItemRequest, HandleRejectMoveItemRequestPacket);
            serverPacketSubject.Subscribe(PacketDefinitions.DrawContainer, HandleDrawContainer);
            serverPacketSubject.Subscribe(PacketDefinitions.PauseClient, HandlePauseClient);
            serverPacketSubject.Subscribe(PacketDefinitions.SendSpeech, HandleSendSpeechPacket);
            serverPacketSubject.Subscribe(PacketDefinitions.StatusBarInfo, HandleStatusBarInfo);
            serverPacketSubject.Subscribe(PacketDefinitions.GraphicalEffect, HandleGraphicalEffect);
            clientPacketSubject.Subscribe(PacketDefinitions.DoubleClick, HandleDoubleClick);

            serverPacketSubject.RegisterOutputFilter(FilterServerPackets);
        }

        private Packet? FilterServerPackets(Packet rawPacket)
        {
            if (itemIdDraggedByScript.HasValue && rawPacket.Id == PacketDefinitions.RejectMoveItemRequest.Id)
            {
                itemIdDraggedByScript = null;
                return null;
            }

            return rawPacket;
        }

        private void HandleGraphicalEffect(GraphicalEffectPacket packet)
        {
            eventJournalSource.Publish(new GraphicalEffectStartedEvent(packet.DirectionType, packet.CharacterId,
                packet.TargetId, packet.Type, packet.Location, packet.TargetLocation));
        }


        private void HandleDoubleClick(DoubleClickRequest request)
        {
            eventJournalSource.Publish(new ItemUseRequestedEvent(request.ItemId));
        }

        private void HandleStatusBarInfo(StatusBarInfoPacket packet)
        {
            if (gameObjects[packet.PlayerId] is Mobile mobile)
            {
                mobile = (Mobile)mobile.UpdateName(packet.PlayerName, packet.CanRename);
                UpdateHealth(mobile, packet.CurrentHealth, packet.MaxHealth);
            }
        }

        private void UpdateHealth(ObjectId id, ushort newHealth, ushort newMaxHealth)
        {
            if (gameObjects[id] is Mobile mobile)
            {
                UpdateHealth(mobile, newHealth, newMaxHealth);
            }
        }

        private void UpdateHealth(Mobile mobile, ushort newHealth, ushort newMaxHealth)
        {
            var oldHealth = mobile.CurrentHealth;

            var updatedItem = mobile.UpdateHealth(newHealth, newMaxHealth);
            gameObjects.UpdateObject(updatedItem);

            if (oldHealth != newHealth)
            {
                eventJournalSource.Publish(new CurrentHealthUpdatedEvent(updatedItem, oldHealth));
            }
        }

        private void HandleSendSpeechPacket(SendSpeechPacket packet)
        {
            var item = gameObjects[packet.Id];

            if (item != null && packet.Name != null && !packet.Name.Equals(item.Name, StringComparison.Ordinal))
                gameObjects.UpdateObject(item.UpdateName(packet.Name));
        }

        private ObjectId? drawContainerId;

        private void HandleDrawContainer(DrawContainerPacket packet)
        {
            drawContainerId = packet.ContainerId;
        }

        private void HandlePauseClient(PauseClientPacket packet)
        {
            // Server resumes client as soons as it is finished with sending content of a container.
            // When container contains an item:
            //      HandleDrawContainer -> HandlePauseClient (Pause) -> HandleAddMultipleItemsInContainer -> HandlePauseClient (Resume)
            // When container is empty:
            //      HandleDrawContainer -> HandlePauseClient (Pause) -> HandlePauseClient (Resume)
            // The problem is with empty containers - no HandleAddMultipleItemsInContainer is invoked
            if (packet.Choice == PauseClientChoice.Resume && drawContainerId.HasValue)
            {
                eventJournalSource.Publish(new ContainerOpenedEvent(drawContainerId.Value));
                drawContainerId = null;
            }
        }

        private void HandleWornItemPacket(WornItemPacket packet)
        {
            gameObjects.UpdateObject(new Item(packet.ItemId, packet.Type, 1, new Location3D(0, 0, 0), packet.Color,
                packet.PlayerId, packet.Layer));

            eventJournalSource.Publish(new ItemWornEvent(packet.ItemId, packet.PlayerId, packet.Layer));
        }

        private void HandleRejectMoveItemRequestPacket(RejectMoveItemRequestPacket packet)
        {
            eventJournalSource.Publish(new MoveItemRequestRejectedEvent((DragResult)packet.Reason));
        }

        private void HandleUpdateCurrentHealthPacket(UpdateCurrentHealthPacket packet)
        {
            UpdateHealth(packet.PlayerId, packet.CurrentHealth, packet.MaxHealth);
        }

        private void HandleUpdatePlayerPacket(UpdatePlayerPacket packet)
        {
            if (packet.PlayerId == legacyApi.Me.PlayerId)
                return;

            if (gameObjects.TryGet(packet.PlayerId, out GameObject existingObject) &&
                existingObject is Mobile existingMobile)
            {
                var updatedMobile = existingMobile.Update(packet.Type, packet.Location, packet.Color,
                    packet.Direction, packet.MovementType, existingMobile.Notoriety, packet.Flags);
                gameObjects.UpdateObject(updatedMobile);

                CheckFlagsChange(existingMobile, updatedMobile);
            }
            else
            {
                gameObjects.UpdateObject(new Mobile(packet.PlayerId, packet.Type, packet.Location, packet.Color,
                    packet.Direction, packet.MovementType, null, packet.Flags));
            }
        }

        private void CheckFlagsChange(Mobile mobile, Mobile updated)
        {
            if (mobile.IsDead != updated.IsDead || mobile.IsPoisoned != updated.IsPoisoned ||
                mobile.IsInWarMode != updated.IsInWarMode)
            {
                eventJournalSource.Publish(new MobileFlagsUpdatedEvent(mobile, updated));
            }
        }

        private void HandleAddItemToContainer(AddItemToContainerPacket packet)
        {
            if (gameObjects.TryGet(packet.ItemId, out GameObject existingObject) && existingObject is Item existingItem)
            {
                gameObjects.UpdateObject(existingItem.Update(packet.Type, packet.Amount, (Location3D) packet.Location,
                    packet.Color,
                    packet.ContainerId, null));
            }
            else
            {
                var item = new Item(packet.ItemId, packet.Type, packet.Amount, (Location3D) packet.Location,
                    packet.Color, packet.ContainerId, null);
                gameObjects.AddObject(item);
                OnItemEnteredView(item);
            }
        }

        private void HandleAddMultipleItemsInContainer(AddMultipleItemsInContainerPacket packet)
        {
            gameObjects.AddItemRange(packet.Items);
        }

        private void HandleDeleteObjectPacket(DeleteObjectPacket packet)
        {
            if (packet.Id == legacyApi.Me.PlayerId)
                return;

            if (itemIdDraggedByScript.HasValue && packet.Id == itemIdDraggedByScript.Value)
                itemIdDraggedByScript = null;

            gameObjects.RemoveItem(packet.Id);
            eventJournalSource.Publish(new ObjectDeletedEvent(packet.Id));
        }

        private void HandleSecondAgeObjectInfoPacket(SecondAgeObjectInformationPacket packet)
        {
            if (gameObjects.TryGet(packet.Id, out var existingObject) && existingObject is Item existingItem)
            {
                gameObjects.UpdateObject(existingItem.Update(packet.Type, packet.Amount, packet.Location,
                    existingItem.Color,
                    existingItem.ContainerId));
            }
            else
            {
                var item = packet.Type == 0x2006
                    ? new Corpse(packet.Id, packet.Type, packet.Amount, packet.Location, packet.Color, null, null)
                    : new Item(packet.Id, packet.Type, packet.Amount, packet.Location, packet.Color, null, null);
                gameObjects.AddObject(item);
                OnItemEnteredView(item);
            }
        }

        private void HandleObjectInfoPacket(ObjectInfoPacket packet)
        {
            if (gameObjects.TryGet(packet.Id, out var existingObject) && existingObject is Item existingItem)
            {
                // ObjectInfo packet holds information about items that are on ground, not in a container, so if
                // an item is in a container and it is updated by a ObjectInfo packet, the item has to be
                // taken out of the container and put on the ground.
                gameObjects.UpdateObject(existingItem.Update(packet.Type, packet.Amount, packet.Location,
                    existingItem.Color,
                    null));
            }
            else
            {
                var item = packet.Type == 0x2006
                    ? new Corpse(packet.Id, packet.Type, packet.Amount, packet.Location, packet.Dye, null, null)
                    : new Item(packet.Id, packet.Type, packet.Amount, packet.Location, packet.Dye, null, null);
                gameObjects.AddObject(item);
                OnItemEnteredView(item);
            }
        }

        private void OnItemEnteredView(Item item)
        {
            eventJournalSource.Publish(new ItemEnteredViewEvent(item));
        }

        private void HandleDrawObjectPacket(DrawObjectPacket packet)
        {
            gameObjects.AddItemRange(packet.Items);

            if (packet.Id == legacyApi.Me.PlayerId)
                return;

            if (gameObjects[packet.Id] is Mobile mobile)
            {
                var updatedMobile = mobile.Update(packet.Type, packet.Location, packet.Color, packet.Direction,
                    packet.MovementType,
                    packet.Notoriety, packet.Flags);

                gameObjects.UpdateObject(updatedMobile);
                CheckFlagsChange(mobile, updatedMobile);
            }
            else
            {
                mobile = new Mobile(packet.Id, packet.Type, packet.Location, packet.Color,
                    packet.Direction, packet.MovementType,
                    packet.Notoriety, packet.Flags);
                gameObjects.AddObject(mobile);
                OnMobileEnteredView(mobile);
            }
        }

        private void OnMobileEnteredView(Mobile mobile)
        {
            eventJournalSource.Publish(new MobileEnteredViewEvent(mobile));
        }

        public void OnPlayerPositionChanged(object sender, Location3D e)
        {
            var newPosition = (Location2D) e;

            gameObjects.PurgeUnreachableItems(newPosition, 25);
        }

        public void DragItem(ObjectId id, int amount)
        {
            itemIdDraggedByScript = id;
            legacyApi.Server.DragItem(id, amount);
        }

        public DragResult WaitForItemDragged(ObjectId? awaitedDragObjectId, TimeSpan? timeout)
        {
            var result = DragResult.None;
            waitForItemDraggedJournal.When<ObjectDeletedEvent>(
                    e => awaitedDragObjectId.HasValue && awaitedDragObjectId.Value == e.DeletedObjectId,
                    e => result = DragResult.Success)
                .When<MoveItemRequestRejectedEvent>(e => result = e.Reason)
                .WhenTimeout(() => result = DragResult.Timeout)
                .WaitAny(timeout);

            return result;
        }
    }
}