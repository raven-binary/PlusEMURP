namespace Plus.Communication.Packets.Outgoing.Rooms.Avatar
{
    public class SleepComposer : MessageComposer
    {
        public int VirtualId { get; }
        public bool IsSleeping { get; }

        public SleepComposer(int virtualId, bool isSleeping)
            : base(ServerPacketHeader.SleepMessageComposer)
        {
            VirtualId = virtualId;
            IsSleeping = isSleeping;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(VirtualId);
            packet.WriteBoolean(IsSleeping);
        }
    }
}