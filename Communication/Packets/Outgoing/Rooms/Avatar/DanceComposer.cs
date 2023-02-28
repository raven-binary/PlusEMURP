namespace Plus.Communication.Packets.Outgoing.Rooms.Avatar
{
    internal class DanceComposer : MessageComposer
    {
        public int VirtualId { get; }
        public int Dance { get; }

        public DanceComposer(int virtualId, int dance)
            : base(ServerPacketHeader.DanceMessageComposer)
        {
            VirtualId = virtualId;
            Dance = dance;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(VirtualId);
            packet.WriteInteger(Dance);
        }
    }
}