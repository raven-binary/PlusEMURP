namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class RoomEntryInfoComposer : MessageComposer
    {
        public int RoomId { get; }
        public bool IsOwner { get; }

        public RoomEntryInfoComposer(int roomId, bool isOwner)
            : base(ServerPacketHeader.RoomEntryInfoMessageComposer)
        {
            RoomId = roomId;
            IsOwner = isOwner;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(RoomId);
            packet.WriteBoolean(IsOwner);
        }
    }
}