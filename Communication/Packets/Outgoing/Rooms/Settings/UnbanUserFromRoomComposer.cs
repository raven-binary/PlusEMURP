namespace Plus.Communication.Packets.Outgoing.Rooms.Settings
{
    internal class UnbanUserFromRoomComposer : MessageComposer
    {
        public int RoomId { get; }
        public int UserId { get; }

        public UnbanUserFromRoomComposer(int roomId, int userId)
            : base(ServerPacketHeader.UnbanUserFromRoomMessageComposer)
        {
            RoomId = roomId;
            UserId = userId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(RoomId);
            packet.WriteInteger(UserId);
        }
    }
}