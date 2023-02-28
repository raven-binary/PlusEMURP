namespace Plus.Communication.Packets.Outgoing.Handshake
{
    public class AuthenticationOkComposer : MessageComposer
    {
        public AuthenticationOkComposer()
            : base(ServerPacketHeader.AuthenticationOkMessageComposer)
        {
        }

        public override void Compose(ServerPacket packet)
        {
        }
    }
}