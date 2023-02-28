namespace Plus.Communication.Packets.Outgoing.Rooms.Session
{
    internal class CloseConnectionComposer : MessageComposer
    {
        public CloseConnectionComposer()
            : base(ServerPacketHeader.CloseConnectionMessageComposer)
        {
        }

        public override void Compose(ServerPacket packet)
        {
        }
    }
}