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
        private readonly ManualResetEvent cannotReachEvent = new ManualResetEvent(false);

        public ItemsObservers(GameObjectCollection gameObjects, IServerPacketSubject serverPacketSubject, IClientPacketSubject clientPacketSubject,
            Legacy legacyApi)
        {
            this.gameObjects = gameObjects;
            this.gameObjects.MobileLeftView += (sender, mobile) => MobileLeftView.RaiseScriptEvent(this, mobile);
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
            serverPacketSubject.Subscribe(PacketDefinitions.ClilocMessage, HandleClilocMessage);
            clientPacketSubject.Subscribe(PacketDefinitions.DoubleClick, HandleDoubleClick);
        }

        private void HandleClilocMessage(ClilocMessagePacket packet)
        {
            if (packet.MessageId == (MessageId) 0x0007A258) // Message "You cannot reach that."
                cannotReachEvent.Set();
        }

        private void HandleDoubleClick(DoubleClickRequest request)
        {
            DoubleClickRequested?.Invoke(this, new ItemUseRequestedArgs(request.ItemId));
        }

        public ObjectId? DraggedItemId { get; set; }

        private void HandleStatusBarInfo(StatusBarInfoPacket packet)
        {
            if (gameObjects[packet.PlayerId] is Mobile mobile)
            {
                mobile = (Mobile)mobile.UpdateName(packet.PlayerName, packet.CanModifyName);
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
                CurrentHealthUpdated.RaiseScriptEvent(this,
                    new CurrentHealthUpdatedArgs(updatedItem, oldHealth));
            }
        }

        internal event EventHandler<CurrentHealthUpdatedArgs> CurrentHealthUpdated;
        internal event EventHandler<ItemUseRequestedArgs> DoubleClickRequested;
        internal event EventHandler<ItemEnteredViewArgs> ItemEnteredView;
        internal event EventHandler<Mobile> MobileEnteredView;
        internal event EventHandler<Mobile> MobileLeftView;

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
                    packet.Direction, packet.MovementType, existingMobile.Notoriety));
            }
            else
            {
                gameObjects.UpdateObject(new Mobile(packet.PlayerId, packet.Type, packet.Location, packet.Color,
                    packet.Direction, packet.MovementType, null));
            }
        }

        internal void ResetEvents()
        {
            CurrentHealthUpdated = null;
            ItemEnteredView = null;
            MobileLeftView = null;
            MobileEnteredView = null;
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
            ItemEnteredView.RaiseScriptEvent(this, new ItemEnteredViewArgs(item));
        }

        private void HandleDrawObjectPacket(DrawObjectPacket packet)
        {
            gameObjects.AddItemRange(packet.Items);

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
            MobileEnteredView.RaiseScriptEvent(this, mobile);
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

        public bool WaitForContainerOpened(TimeSpan? timeout)
        {
            var timeSpentWaiting = new TimeSpan();
            var sleepSpan = TimeSpan.FromMilliseconds(100);

            resumeClientReceivedEvent.Reset();
            drawContainerReceivedEvent.Reset();
            cannotReachEvent.Reset();

            int waitAnyResult;
            while ((waitAnyResult = WaitHandle.WaitAny(new WaitHandle[] { drawContainerReceivedEvent, cannotReachEvent }, sleepSpan)) == WaitHandle.WaitTimeout)
            {
                legacyApi.CheckCancellation();

                if (timeout.HasValue)
                {
                    timeSpentWaiting += sleepSpan;
                    if (timeSpentWaiting > timeout.Value)
                        throw new TimeoutException();
                }
            }

            if (waitAnyResult == 1 /* index of cannotReachEvent */)
                return false;

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

            return true;
        }
    }
}