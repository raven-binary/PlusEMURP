using Plus.HabboHotel.Items;

namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class ObjectRemoveComposer : MessageComposer
    {
        public Item Item { get; }
        public int UserId { get; }

        public ObjectRemoveComposer(Item item, int userId)
            : base(ServerPacketHeader.ObjectRemoveMessageComposer)
        {
            Item = item;
            UserId = userId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(Item.Id.ToString());
            packet.WriteBoolean(false);
            packet.WriteInteger(UserId);
            packet.WriteInteger(0);
        }
    }
}