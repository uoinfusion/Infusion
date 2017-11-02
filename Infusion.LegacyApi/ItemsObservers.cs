using System;
using System.Threading;
using System.Threading.Tasks;
using Infusion.LegacyApi.Events;
using Infusion.Packets;
using Infusion.Packets.Client;
using Infusion.Packets.Server;

namespace Infusion.LegacyApi
{
    internal class ItemsObservers
    {
        private readonly GameObjectCollection gameObjects;
        private readonly ManualResetEvent itemDragResultReceived = new ManualResetEvent(false);
        private readonly Legacy legacyApi;
        private readonly EventJournalSource eventJournalSource;

        private DragResult dragResult = DragResult.None;

        public ItemsObservers(GameObjectCollection gameObjects, IServerPacketSubject serverPacketSubject, IClientPacketSubject clientPacketSubject,
            Legacy legacyApi, EventJournalSource eventJournalSource)
        {
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
            serverPacketSubject.Subscribe(PacketDefinitions.DrawObject, HandleDrawObjectPacket);
            serverPacketSubject.Subscribe(PacketDefinitions.UpdatePlayer, HandleUpdatePlayerPacket);
            serverPacketSubject.Subscribe(PacketDefinitions.UpdateCurrentHealth, HandleUpdateCurrentHealthPacket);
            serverPacketSubject.Subscribe(PacketDefinitions.WornItem, HandleWornItemPacket);
            serverPacketSubject.Subscribe(PacketDefinitions.RejectMoveItemRequest, HandleRejectMoveItemRequestPacket);
            serverPacketSubject.Subscribe(PacketDefinitions.DrawContainer, HandleDrawContainer);
            serverPacketSubject.Subscribe(PacketDefinitions.PauseClient, HandlePauseClient);
            serverPacketSubject.Subscribe(PacketDefinitions.SendSpeech, HandleSendSpeechPacket);
            serverPacketSubject.Subscribe(PacketDefinitions.StatusBarInfo, HandleStatusBarInfo);
            clientPacketSubject.Subscribe(PacketDefinitions.DoubleClick, HandleDoubleClick);
        }

        private void HandleDoubleClick(DoubleClickRequest request)
        {
            eventJournalSource.Publish(new ItemUseRequestedEvent(request.ItemId));
        }

        public ObjectId? DraggedItemId { get; set; }

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
        }

        private void HandleRejectMoveItemRequestPacket(RejectMoveItemRequestPacket packet)
        {
            DraggedItemId = null;
            dragResult = (DragResult) packet.Reason;
            itemDragResultReceived.Set();
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
                gameObjects.UpdateObject(existingMobile.Update(packet.Type, packet.Location, packet.Color,
                    packet.Direction, packet.MovementType, existingMobile.Notoriety));
            }
            else
            {
                gameObjects.UpdateObject(new Mobile(packet.PlayerId, packet.Type, packet.Location, packet.Color,
                    packet.Direction, packet.MovementType, null));
            }
        }

        private void HandleAddItemToContainer(AddItemToContainerPacket packet)
        {
            if (gameObjects.TryGet(packet.ItemId, out GameObject existingObject) && existingObject is Item existingItem)
            {
                gameObjects.UpdateObject(existingItem.Update(packet.Type, packet.Amount, (Location3D) packet.Location,
                    packet.Color,
                    packet.ContainerId));
            }
            else
            {
                gameObjects.AddObject(new Item(packet.ItemId, packet.Type, packet.Amount, (Location3D) packet.Location,
                    packet.Color, packet.ContainerId, null));
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

            var itemId = DraggedItemId;

            if (itemId.HasValue && packet.Id == itemId.Value)
            {
                DraggedItemId = null;
                dragResult = DragResult.Success;
                itemDragResultReceived.Set();
            }

            gameObjects.RemoveItem(packet.Id);
        }

        private void HandleObjectInfoPacket(ObjectInfoPacket packet)
        {
            if (gameObjects.TryGet(packet.Id, out GameObject existingObject) && existingObject is Item existingItem)
            {
                gameObjects.UpdateObject(existingItem.Update(packet.Type, packet.Amount, packet.Location,
                    existingItem.Color,
                    existingItem.ContainerId));
            }
            else
            {
                var item = new Item(packet.Id, packet.Type, packet.Amount, packet.Location, packet.Dye, null, null);
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
                gameObjects.UpdateObject(mobile.Update(packet.Type, packet.Location, packet.Color, packet.Direction, packet.MovementType,
                    packet.Notoriety));
            else
            {
                mobile = new Mobile(packet.Id, packet.Type, packet.Location, packet.Color,
                    packet.Direction, packet.MovementType,
                    packet.Notoriety);
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

        public DragResult WaitForItemDragged(TimeSpan? timeout)
        {
            dragResult = DragResult.None;
            itemDragResultReceived.Reset();

            var timeSpentWaiting = new TimeSpan();
            var sleepSpan = TimeSpan.FromMilliseconds(100);

            while (!itemDragResultReceived.WaitOne(sleepSpan))
            {
                legacyApi.CheckCancellation();

                if (timeout.HasValue)
                {
                    timeSpentWaiting += sleepSpan;
                    if (timeSpentWaiting > timeout.Value)
                        return DragResult.Timeout;
                }
            }

            return dragResult;
        }
    }
}