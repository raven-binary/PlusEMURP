namespace Plus.Communication.Packets.Outgoing.Rooms.Notifications
{
    internal class RoomErrorNotifComposer : MessageComposer
    {
        public int Error { get; }

        public RoomErrorNotifComposer(int error)
            : base(ServerPacketHeader.RoomErrorNotifMessageComposer)
        {
            Error = error;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Error);
        }
    }
}