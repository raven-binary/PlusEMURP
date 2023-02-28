using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Outgoing.Rooms.Permissions
{
    internal class YouAreControllerComposer : MessageComposer
    {
        public int Setting { get; }

        public YouAreControllerComposer(RoomRightLevels level)
            : base(ServerPacketHeader.YouAreControllerMessageComposer)
        {
            Setting = (int)level;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Setting);
        }
    }
}