namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class UserRemoveComposer : MessageComposer
    {
        public int UserId { get; }

        public UserRemoveComposer(int id)
            : base(ServerPacketHeader.UserRemoveMessageComposer)
        {
            UserId = id;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(UserId.ToString());
        }
    }
}