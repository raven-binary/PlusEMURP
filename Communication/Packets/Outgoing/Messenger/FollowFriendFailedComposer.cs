namespace Plus.Communication.Packets.Outgoing.Messenger
{
    internal class FollowFriendFailedComposer : MessageComposer
    {
        public int ErrorCode { get; }

        public FollowFriendFailedComposer(int errorCode)
            : base(ServerPacketHeader.FollowFriendFailedMessageComposer)
        {
            ErrorCode = errorCode;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(ErrorCode);
        }
    }
}