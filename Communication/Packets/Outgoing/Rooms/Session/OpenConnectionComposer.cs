namespace Plus.Communication.Packets.Outgoing.Rooms.Session
{
    internal class OpenConnectionComposer : MessageComposer
    {
        public OpenConnectionComposer()
            : base(ServerPacketHeader.OpenConnectionMessageComposer)
        {
        }

        public override void Compose(ServerPacket packet)
        {
        }
    }
}