namespace Plus.Communication.Packets.Outgoing.Rooms.Settings
{
    internal class RoomSettingsSavedComposer : MessageComposer
    {
        public int RoomId { get; }

        public RoomSettingsSavedComposer(int roomId)
            : base(ServerPacketHeader.RoomSettingsSavedMessageComposer)
        {
            RoomId = roomId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(RoomId);
        }
    }
}