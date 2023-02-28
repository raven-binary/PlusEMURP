namespace Plus.Communication.Packets.Outgoing.Rooms.Chat
{
    public class UserTypingComposer : MessageComposer
    {
        public int VirtualId { get; }
        public bool Typing { get; }

        public UserTypingComposer(int virtualId, bool typing)
            : base(ServerPacketHeader.UserTypingMessageComposer)
        {
            VirtualId = virtualId;
            Typing = typing;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(VirtualId);
            packet.WriteInteger(Typing ? 1 : 0);
        }
    }
}