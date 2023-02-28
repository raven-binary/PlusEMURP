namespace Plus.Communication.Packets.Outgoing.Rooms.Settings
{
    internal class RoomMuteSettingsComposer : MessageComposer
    {
        public bool Status { get; }

        public RoomMuteSettingsComposer(bool status)
            : base(ServerPacketHeader.RoomMuteSettingsMessageComposer)
        {
            Status = status;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteBoolean(Status);
        }
    }
}