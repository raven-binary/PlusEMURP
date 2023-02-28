namespace Plus.Communication.Packets.Outgoing.Groups
{
    internal class RefreshFavouriteGroupComposer : MessageComposer
    {
        public int GroupId { get; }

        public RefreshFavouriteGroupComposer(int id)
            : base(ServerPacketHeader.RefreshFavouriteGroupMessageComposer)
        {
            GroupId = id;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Id);
        }
    }
}