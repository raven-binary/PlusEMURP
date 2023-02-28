namespace Plus.Communication.Packets.Outgoing.Messenger
{
    internal class RoomInviteComposer : MessageComposer
    {
        public int SenderId { get; }
        public string Text { get; }

        public RoomInviteComposer(int senderId, string text)
            : base(ServerPacketHeader.RoomInviteMessageComposer)
        {
            SenderId = senderId;
            Text = text;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(SenderId);
            packet.WriteString(Text);
        }
    }
}