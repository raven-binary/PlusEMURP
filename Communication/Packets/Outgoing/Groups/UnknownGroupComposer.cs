namespace Plus.Communication.Packets.Outgoing.Groups
{
    internal class UnknownGroupComposer : MessageComposer
    {
        public int GroupId { get; }
        public int HabboId { get; }

        public UnknownGroupComposer(int groupId, int habboId)
            : base(ServerPacketHeader.UnknownGroupMessageComposer)
        {
            GroupId = groupId;
            HabboId = habboId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(GroupId);
            packet.WriteInteger(HabboId);
        }
    }
}