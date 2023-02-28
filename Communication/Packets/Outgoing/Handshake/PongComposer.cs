namespace Plus.Communication.Packets.Outgoing.Handshake
{
    internal class PongComposer : MessageComposer
    {
        public PongComposer()
            : base(ServerPacketHeader.PongMessageComposer)
        {
        }

        public override void Compose(ServerPacket packet)
        {
        }
    }
}