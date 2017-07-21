using System;
using System.Threading;
using System.Threading.Tasks;
using Infusion.Packets;
using Infusion.Packets.Server;

namespace Infusion.LegacyApi
{
    internal class ItemsObservers
    {
        private readonly ManualResetEvent drawContainerReceivedEvent = new ManualResetEvent(false);
        private readonly ManualResetEvent itemDragResultReceived = new ManualResetEvent(false);
        private readonly ItemCollection items;
        private readonly Legacy legacyApi;

        private readonly ManualResetEvent resumeClientReceivedEvent = new ManualResetEvent(false);
        private DragResult dragResult = DragResult.None;

        public ItemsObservers(ItemCollection items, IServerPacketSubject serverPacketSubject, Legacy legacyApi)
        {
            this.items = items;
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
        }

        public uint? DraggedItemId { get; set; }

        private void HandleStatusBarInfo(StatusBarInfoPacket packet)
        {
            UpdateHealth(packet.PlayerId, packet.CurrentHealth, packet.MaxHealth);
        }

        private void UpdateHealth(uint id, ushort newHealth, ushort newMaxHealth)
        {
            var item = items[id];
            if (item != null)
            {
                var oldHealth = item.CurrentHealth;

                var updatedItem = item.UpdateHealth(newHealth, newMaxHealth);
                items.UpdateItem(updatedItem);

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
        internal event EventHandler<ItemEnteredViewArgs> ItemEnteredView;

        private void HandleSendSpeechPacket(SendSpeechPacket packet)
        {
            var item = items[packet.Id];

            if (item != null && packet.Name != null && !packet.Name.Equals(item.Name, StringComparison.Ordinal))
                items.UpdateItem(item.UpdateName(packet.Name));
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
            items.UpdateItem(new Item(packet.ItemId, packet.Type, 1, new Location3D(0, 0, 0), packet.Color,
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
            Item existingItem;
            if (items.TryGet(packet.PlayerId, out existingItem))
                items.UpdateItem(existingItem.Update(packet.Type, 1, packet.Location, packet.Color, null));
            else
            {
                items.UpdateItem(new Item(packet.PlayerId, packet.Type, 1, packet.Location, packet.Color,
                    orientation: packet.Direction));
            }
        }

        internal void ResetEvents()
        {
            CurrentHealthUpdated = null;
            ItemEnteredView = null;
        }

        private void HandleAddItemToContainer(AddItemToContainerPacket packet)
        {
            Item existingItem;
            if (items.TryGet(packet.ItemId, out existingItem))
            {
                items.UpdateItem(existingItem.Update(packet.Type, packet.Amount, (Location3D) packet.Location,
                    packet.Color,
                    packet.ContainerId));
            }
            else
            {
                items.AddItem(new Item(packet.ItemId, packet.Type, packet.Amount, (Location3D) packet.Location,
                    packet.Color, packet.ContainerId));
            }
        }

        private void HandleAddMultipleItemsInContainer(AddMultipleItemsInContainerPacket packet)
        {
            items.AddItemRange(packet.Items);
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

            items.RemoveItem(packet.Id);
        }

        private void HandleObjectInfoPacket(ObjectInfoPacket packet)
        {
            if (items.TryGet(packet.Id, out Item existingItem))
            {
                items.UpdateItem(existingItem.Update(packet.Type, packet.Amount, packet.Location, existingItem.Color,
                    existingItem.ContainerId));
            }
            else
            {
                var item = new Item(packet.Id, packet.Type, packet.Amount, packet.Location);
                items.AddItem(item);
                OnItemEnteredView(item);
            }
        }

        private void OnItemEnteredView(Item item)
        {
            if (ItemEnteredView != null)
            {
                Task.Run(() => { ItemEnteredView?.Invoke(this, new ItemEnteredViewArgs(item)); });
            }
            ;
        }

        private void HandleDrawObjectPacket(DrawObjectPacket packet)
        {
            items.AddItemRange(packet.Items);

            var item = items[packet.Id];
            if (item != null)
                items.UpdateItem(item.Update(packet.Type, 1, packet.Location, packet.Color, null, packet.Notoriety));
            else
            {
                items.AddItem(new Item(packet.Id, packet.Type, 1, packet.Location, packet.Color,
                    notoriety: packet.Notoriety));
            }
        }

        public void OnPlayerPositionChanged(object sender, Location3D e)
        {
            var newPosition = (Location2D) e;

            items.PurgeUnreachableItems(newPosition, 25);
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