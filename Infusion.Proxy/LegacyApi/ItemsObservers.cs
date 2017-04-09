using Infusion.Packets;
using Infusion.Packets.Server;

namespace Infusion.Proxy.LegacyApi
{
    internal class ItemsObservers
    {
        private readonly ItemCollection items;
        private Location2D? lastItemPurgeLocation;

        public ItemsObservers(ItemCollection items, ServerPacketHandler serverPacketHandler)
        {
            this.items = items;
            serverPacketHandler.Subscribe(PacketDefinitions.AddMultipleItemsInContainer,
                HandleAddMultipleItemsInContainer);
            serverPacketHandler.Subscribe(PacketDefinitions.AddItemToContainer, HandleAddItemToContainer);
            serverPacketHandler.Subscribe(PacketDefinitions.DeleteObject, HandleDeleteObjectPacket);
            serverPacketHandler.Subscribe(PacketDefinitions.ObjectInfo, HandleObjectInfoPacket);
            serverPacketHandler.Subscribe(PacketDefinitions.DrawObject, HandleDrawObjectPacket);
            serverPacketHandler.Subscribe(PacketDefinitions.UpdatePlayer, HandleUpdatePlayerPacket);
            serverPacketHandler.Subscribe(PacketDefinitions.UpdateCurrentHealth, HandleUpdateCurrentHealthPacket);
            serverPacketHandler.Subscribe(PacketDefinitions.WornItem, HandleWornItemPacket);
        }

        public bool HitPointNotificationEnabled { get; set; }

        private void HandleWornItemPacket(WornItemPacket packet)
        {
            items.UpdateItem(new Item(packet.ItemId, packet.Type, 1, new Location3D(0, 0, 0), packet.Color,
                packet.PlayerId, packet.Layer));
        }

        private void HandleUpdateCurrentHealthPacket(UpdateCurrentHealthPacket packet)
        {
            Item item;
            if (items.TryGet(packet.PlayerId, out item))
            {
                if (HitPointNotificationEnabled)
                {
                    var delta = packet.CurrentHealth - item.CurrentHealth;
                    if (delta != 0)
                    {
                        var deltaText = item.CurrentHealth == 0 ? "??" : delta.ToString();
                        var color = delta > 0 ? Colors.Blue : Colors.Green;
                        Legacy.ClientPrint($"{deltaText} - [{packet.CurrentHealth}/{packet.MaxHealth}]", "update",
                            item.Id,
                            item.Type, SpeechType.Speech, color);
                    }
                }

                items.UpdateItem(item.UpdateHealth(packet.CurrentHealth, packet.MaxHealth));
            }
        }

        private void HandleUpdatePlayerPacket(UpdatePlayerPacket packet)
        {
            Item existingItem;
            if (items.TryGet(packet.PlayerId, out existingItem))
                items.UpdateItem(existingItem.Update(1, packet.Location, packet.Color, null));
            else
            {
                items.UpdateItem(new Item(packet.PlayerId, packet.Type, 1, packet.Location, packet.Color,
                    orientation: packet.Direction));
            }
        }

        private void HandleAddItemToContainer(AddItemToContainerPacket packet)
        {
            Item existingItem;
            if (items.TryGet(packet.ItemId, out existingItem))
            {
                items.UpdateItem(existingItem.Update(packet.Amount, (Location3D) packet.Location, packet.Color,
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
            items.RemoveItem(packet.Id);
        }

        private void HandleObjectInfoPacket(ObjectInfoPacket packet)
        {
            if (items.TryGet(packet.Id, out Item existingItem))
                items.UpdateItem(existingItem.Update(packet.Amount, packet.Location, existingItem.Color,
                    existingItem.ContainerId));
            else
            {
                items.AddItem(new Item(packet.Id, packet.Type, packet.Amount,
                    packet.Location));
            }
        }

        private void HandleDrawObjectPacket(DrawObjectPacket packet)
        {
            items.AddItemRange(packet.Items);

            var item = items[packet.Id];
            if (item != null)
                items.UpdateItem(item.Update(1, packet.Location, packet.Color, null, packet.Notoriety));
            else
                items.AddItem(new Item(packet.Id, packet.Type, 1, packet.Location, packet.Color, notoriety: packet.Notoriety));
        }

        public void OnPlayerPositionChanged(object sender, Location3D e)
        {
            var newPosition = (Location2D) e;

            if (lastItemPurgeLocation.HasValue)
            {
                if (lastItemPurgeLocation.Value.GetDistance(newPosition) > 200)
                {
                    items.PurgeUnreachableItems(newPosition, 200);
                    lastItemPurgeLocation = newPosition;
                }
            }
            else
                lastItemPurgeLocation = newPosition;
        }

        public void Ignore(Item item)
        {
            items.UpdateItem(item.Ignore());
        }
    }
}