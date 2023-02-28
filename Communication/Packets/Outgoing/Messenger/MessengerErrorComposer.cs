namespace Plus.Communication.Packets.Outgoing.Messenger
{
    internal class MessengerErrorComposer : MessageComposer
    {
        public int ErrorCode1 { get; }
        public int ErrorCode2 { get; }

        public MessengerErrorComposer(int errorCode1, int errorCode2)
            : base(ServerPacketHeader.MessengerErrorMessageComposer)
        {
            ErrorCode1 = errorCode1;
            ErrorCode2 = errorCode2;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(ErrorCode1);
            packet.WriteInteger(ErrorCode2);
        }
    }
}