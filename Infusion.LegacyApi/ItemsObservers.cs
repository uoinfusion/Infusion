using System;
using System.Threading;
using System.Threading.Tasks;
using Infusion.Packets;
using Infusion.Packets.Client;
using Infusion.Packets.Server;

namespace Infusion.LegacyApi
{
    internal class ItemsObservers
    {
        private readonly ManualResetEvent drawContainerReceivedEvent = new ManualResetEvent(false);
        private readonly GameObjectCollection gameObjects;
        private readonly ManualResetEvent itemDragResultReceived = new ManualResetEvent(false);
        private readonly Legacy legacyApi;

        private readonly ManualResetEvent resumeClientReceivedEvent = new ManualResetEvent(false);
        private DragResult dragResult = DragResult.None;

        public ItemsObservers(GameObjectCollection gameObjects, IServerPacketSubject serverPacketSubject, IClientPacketSubject clientPacketSubject,
            Legacy legacyApi)
        {
            this.gameObjects = gameObjects;
            this.legacyApi = legacyApi;
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
            DoubleClickRequested?.Invoke(this, new ItemUseRequestedArgs(request.ItemId));
        }

        public ObjectId? DraggedItemId { get; set; }

        private void HandleStatusBarInfo(StatusBarInfoPacket packet)
        {
            UpdateHealth(packet.PlayerId, packet.CurrentHealth, packet.MaxHealth);
        }

        private void UpdateHealth(ObjectId id, ushort newHealth, ushort newMaxHealth)
        {
            var mobile = gameObjects[id] as Mobile;
            if (mobile != null)
            {
                var oldHealth = mobile.CurrentHealth;

                var updatedItem = mobile.UpdateHealth(newHealth, newMaxHealth);
                gameObjects.UpdateObject(updatedItem);

                if (oldHealth != newHealth && CurrentHealthUpdated != null)
                {
                    Task.Run(() =>
                    {
                        CurrentHealthUpdated?.Invoke(this,
                            new CurrentHealthUpdatedArgs(updatedItem, oldHealth));
                    });
                }
            }
        }

        internal event EventHandler<CurrentHealthUpdatedArgs> CurrentHealthUpdated;
        internal event EventHandler<ItemUseRequestedArgs> DoubleClickRequested;
        internal event EventHandler<ItemEnteredViewArgs> ItemEnteredView;

        private void HandleSendSpeechPacket(SendSpeechPacket packet)
        {
            var item = gameObjects[packet.Id];

            if (item != null && packet.Name != null && !packet.Name.Equals(item.Name, StringComparison.Ordinal))
                gameObjects.UpdateObject(item.UpdateName(packet.Name));
        }

        private void HandlePauseClient(PauseClientPacket packet)
        {
            if (packet.Choice == PauseClientChoice.Resume)
                resumeClientReceivedEvent.Set();
        }

        private void HandleDrawContainer(DrawContainerPacket packet)
        {
            // ignoring possibility that there might be many container being openned at the same time
            drawContainerReceivedEvent.Set();
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
            if (gameObjects.TryGet(packet.PlayerId, out GameObject existingObject) &&
                existingObject is Mobile existingMobile)
            {
                gameObjects.UpdateObject(existingMobile.Update(packet.Type, packet.Location, packet.Color,
                    packet.Direction, existingMobile.Notoriety));
            }
            else
            {
                gameObjects.UpdateObject(new Mobile(packet.PlayerId, packet.Type, packet.Location, packet.Color,
                    packet.Direction, null));
            }
        }

        internal void ResetEvents()
        {
            CurrentHealthUpdated = null;
            ItemEnteredView = null;
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
            if (ItemEnteredView != null)
                Task.Run(() => { ItemEnteredView?.Invoke(this, new ItemEnteredViewArgs(item)); });
            ;
        }

        private void HandleDrawObjectPacket(DrawObjectPacket packet)
        {
            gameObjects.AddItemRange(packet.Items);

            var mobile = gameObjects[packet.Id] as Mobile;
            if (mobile != null)
                gameObjects.UpdateObject(mobile.Update(packet.Type, packet.Location, packet.Color, packet.Direction,
                    packet.Notoriety));
            else
            {
                gameObjects.AddObject(new Mobile(packet.Id, packet.Type, packet.Location, packet.Color,
                    packet.Direction,
                    packet.Notoriety));
            }
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

        public void WaitForContainerOpened(TimeSpan? timeout)
        {
            var timeSpentWaiting = new TimeSpan();
            var sleepSpan = TimeSpan.FromMilliseconds(100);

            resumeClientReceivedEvent.Reset();
            drawContainerReceivedEvent.Reset();
            while (!drawContainerReceivedEvent.WaitOne(sleepSpan))
            {
                legacyApi.CheckCancellation();

                if (timeout.HasValue)
                {
                    timeSpentWaiting += sleepSpan;
                    if (timeSpentWaiting > timeout.Value)
                        throw new TimeoutException();
                }
            }

            while (!resumeClientReceivedEvent.WaitOne(sleepSpan))
            {
                legacyApi.CheckCancellation();

                if (timeout.HasValue)
                {
                    timeSpentWaiting += sleepSpan;
                    if (timeSpentWaiting > timeout.Value)
                        throw new TimeoutException();
                }
            }
        }
    }
}