namespace Plus.Communication.Packets.Outgoing.Rooms.Settings
{
    internal class FlatControllerAddedComposer : MessageComposer
    {
        public int RoomId { get; }
        public int UserId { get; }
        public string Username { get; }

        public FlatControllerAddedComposer(int roomId, int userId, string username)
            : base(ServerPacketHeader.FlatControllerAddedMessageComposer)
        {
            RoomId = roomId;
            UserId = userId;
            Username = username;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(RoomId);
            packet.WriteInteger(UserId);
            packet.WriteString(Username);
        }
    }
}