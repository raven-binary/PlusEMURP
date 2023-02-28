namespace Plus.Communication.Packets.Outgoing.Groups
{
    internal class NewGroupInfoComposer : MessageComposer
    {
        public int RoomId { get; }
        public int GroupId { get; }

        public NewGroupInfoComposer(int roomId, int groupId)
            : base(ServerPacketHeader.NewGroupInfoMessageComposer)
        {
            RoomId = roomId;
            GroupId = groupId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(RoomId);
            packet.WriteInteger(GroupId);
        }
    }
}