namespace Plus.Communication.Packets.Outgoing.Handshake
{
    internal class GenericErrorComposer : MessageComposer
    {
        public int ErrorId { get; }

        public GenericErrorComposer(int errorId)
            : base(ServerPacketHeader.GenericErrorMessageComposer)
        {
            ErrorId = errorId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(ErrorId);
        }
    }
}