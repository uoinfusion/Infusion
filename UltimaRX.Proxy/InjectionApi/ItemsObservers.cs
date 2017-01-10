using UltimaRX.Packets;
using UltimaRX.Packets.Server;

namespace UltimaRX.Proxy.InjectionApi
{
    internal class ItemsObservers
    {
        private readonly ItemCollection collection;

        private ItemCollection items = new ItemCollection();

        public ItemsObservers(ItemCollection collection, ServerPacketHandler serverPacketHandler)
        {
            this.collection = collection;
            serverPacketHandler.Subscribe(PacketDefinitions.AddMultipleItemsInContainer,
                HandleAddMultipleItemsInContainer);
            serverPacketHandler.Subscribe(PacketDefinitions.AddItemToContainer, HandleAddItemToContainer);
            serverPacketHandler.Subscribe(PacketDefinitions.DeleteObject, HandleDeleteObjectPacket);
            serverPacketHandler.Subscribe(PacketDefinitions.ObjectInfo, HandleObjectInfoPacket);
            serverPacketHandler.Subscribe(PacketDefinitions.DrawObject, HandleDrawObjectPacket);
            serverPacketHandler.Subscribe(PacketDefinitions.UpdatePlayer, HandleUpdatePlayerPacket);
        }

        private void HandleUpdatePlayerPacket(UpdatePlayerPacket packet)
        {
            collection.UpdateItem(new Item(packet.PlayerId, packet.Type, 1, packet.Location, packet.Color,
                orientation: packet.Direction));
        }

        private void HandleAddItemToContainer(AddItemToContainerPacket packet)
        {
            collection.AddItem(new Item(packet.ItemId, packet.Type, packet.Amount, (Location3D) packet.Location,
                packet.Color, packet.ContainerId));
        }

        private void HandleAddMultipleItemsInContainer(AddMultipleItemsInContainerPacket packet)
        {
            collection.AddItemRange(packet.Items);
        }

        private void HandleDeleteObjectPacket(DeleteObjectPacket packet)
        {
            collection.RemoveItem(packet.Id);
        }

        private void HandleObjectInfoPacket(ObjectInfoPacket packet)
        {
            var item = new Item(packet.Id, packet.Type, packet.Amount,
                packet.Location);
            collection.AddItem(item);
        }

        private void HandleDrawObjectPacket(DrawObjectPacket packet)
        {
            collection.AddItemRange(packet.Items);
            collection.AddItem(new Item(packet.Id, packet.Type, 1,
                packet.Location, packet.Color));
        }
    }
}