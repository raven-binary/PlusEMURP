namespace Plus.Communication.Packets.Outgoing.Navigator
{
    internal class NavigatorSettingsComposer : MessageComposer
    {
        public int HomeRoomId { get; }

        public NavigatorSettingsComposer(int homeroom)
            : base(ServerPacketHeader.NavigatorSettingsMessageComposer)
        {
            HomeRoomId = homeroom;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(HomeRoomId);
            packet.WriteInteger(HomeRoomId);
        }
    }
}