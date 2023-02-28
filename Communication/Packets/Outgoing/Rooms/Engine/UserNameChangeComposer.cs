namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class UserNameChangeComposer : MessageComposer
    {
        public int RoomId { get; }
        public int VirtualId { get; }
        public string Username { get; }

        public UserNameChangeComposer(int roomId, int virtualId, string username)
            : base(ServerPacketHeader.UserNameChangeMessageComposer)
        {
            RoomId = roomId;
            VirtualId = virtualId;
            Username = username;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(RoomId);
            packet.WriteInteger(VirtualId);
            packet.WriteString(Username);
        }
    }
}