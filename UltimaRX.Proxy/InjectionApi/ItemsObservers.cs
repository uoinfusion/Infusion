using UltimaRX.Packets;
using UltimaRX.Packets.Server;

namespace UltimaRX.Proxy.InjectionApi
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
        }

        private void HandleUpdateCurrentHealthPacket(UpdateCurrentHealthPacket packet)
        {
            Item item;
            if (items.TryGet(packet.PlayerId, out item))
            {
                items.UpdateItem(item.UpdateHealth(packet.CurrentHealth, packet.MaxHealth));
            }
        }

        private void HandleUpdatePlayerPacket(UpdatePlayerPacket packet)
        {
            items.UpdateItem(new Item(packet.PlayerId, packet.Type, 1, packet.Location, packet.Color,
                orientation: packet.Direction));
        }

        private void HandleAddItemToContainer(AddItemToContainerPacket packet)
        {
            items.AddItem(new Item(packet.ItemId, packet.Type, packet.Amount, (Location3D) packet.Location,
                packet.Color, packet.ContainerId));
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
            var item = new Item(packet.Id, packet.Type, packet.Amount,
                packet.Location);
            items.AddItem(item);
        }

        private void HandleDrawObjectPacket(DrawObjectPacket packet)
        {
            items.AddItemRange(packet.Items);
            items.AddItem(new Item(packet.Id, packet.Type, 1,
                packet.Location, packet.Color));
        }

        public void OnPlayerPositionChanged(object sender, Location3D e)
        {
            var newPosition = (Location2D) e;

            if (lastItemPurgeLocation.HasValue)
            {
                if (lastItemPurgeLocation.Value.GetDistance(newPosition) > 50)
                {
                    items.PurgeUnreachableItems(newPosition, 50);
                    lastItemPurgeLocation = newPosition;
                }
            }
            else
            {
                lastItemPurgeLocation = newPosition;
            }
        }
    }
}