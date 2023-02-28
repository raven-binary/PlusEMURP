using Plus.HabboHotel.Items;

namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class ItemRemoveComposer : MessageComposer
    {
        public int ItemId { get; }
        public int UserId { get; }

        public ItemRemoveComposer(Item item, int userId)
            : base(ServerPacketHeader.ItemRemoveMessageComposer)
        {
            ItemId = item.Id;
            UserId = userId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(ItemId.ToString());
            packet.WriteBoolean(false);
            packet.WriteInteger(UserId);
        }
    }
}