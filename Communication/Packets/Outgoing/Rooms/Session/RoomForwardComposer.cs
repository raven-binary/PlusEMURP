namespace Plus.Communication.Packets.Outgoing.Rooms.Session
{
    public class RoomForwardComposer : MessageComposer
    {
        public int RoomId { get; }

        public RoomForwardComposer(int roomId)
            : base(ServerPacketHeader.RoomForwardMessageComposer)
        {
            RoomId = roomId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(RoomId);
        }
    }
}