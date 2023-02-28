namespace Plus.Communication.Packets.Outgoing.Navigator
{
    internal class RoomInfoUpdatedComposer : MessageComposer
    {
        public int RoomId { get; }

        public RoomInfoUpdatedComposer(int roomId)
            : base(ServerPacketHeader.RoomInfoUpdatedMessageComposer)
        {
            RoomId = roomId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(RoomId);
        }
    }
}