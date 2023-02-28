namespace Plus.Communication.Packets.Outgoing.Rooms.Settings
{
    internal class FlatControllerRemovedComposer : MessageComposer
    {
        public int RoomId { get; }
        public int UserId { get; }

        public FlatControllerRemovedComposer(int roomId, int userId)
            : base(ServerPacketHeader.FlatControllerRemovedMessageComposer)
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