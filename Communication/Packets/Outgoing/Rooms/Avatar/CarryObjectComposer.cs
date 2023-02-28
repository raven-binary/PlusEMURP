namespace Plus.Communication.Packets.Outgoing.Rooms.Avatar
{
    internal class CarryObjectComposer : MessageComposer
    {
        public int VirtualId { get; }
        public int ItemId { get; }

        public CarryObjectComposer(int virtualId, int itemId)
            : base(ServerPacketHeader.CarryObjectMessageComposer)
        {
            VirtualId = virtualId;
            ItemId = itemId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(VirtualId);
            packet.WriteInteger(ItemId);
        }
    }
}