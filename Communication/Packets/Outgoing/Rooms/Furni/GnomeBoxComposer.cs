namespace Plus.Communication.Packets.Outgoing.Rooms.Furni
{
    internal class GnomeBoxComposer : MessageComposer
    {
        public int ItemId { get; }

        public GnomeBoxComposer(int itemId)
            : base(ServerPacketHeader.GnomeBoxMessageComposer)
        {
            ItemId = itemId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(ItemId);
        }
    }
}