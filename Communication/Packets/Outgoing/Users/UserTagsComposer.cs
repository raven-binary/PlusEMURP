namespace Plus.Communication.Packets.Outgoing.Users
{
    internal class UserTagsComposer : MessageComposer
    {
        public int UserId { get; }

        public UserTagsComposer(int userId)
            : base(ServerPacketHeader.UserTagsMessageComposer)
        {
            UserId = userId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(UserId);
            packet.WriteInteger(0); // tag count
            {
                // append each tag as a string
            }
        }
    }
}