namespace Plus.Communication.Packets.Outgoing.Navigator
{
    internal class FlatCreatedComposer : MessageComposer
    {
        public int RoomId { get; }
        public string RoomName { get; }

        public FlatCreatedComposer(int roomId, string roomName)
            : base(ServerPacketHeader.FlatCreatedMessageComposer)
        {
            RoomId = roomId;
            RoomName = roomName;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(RoomId);
            packet.WriteString(RoomName);
        }
    }
}