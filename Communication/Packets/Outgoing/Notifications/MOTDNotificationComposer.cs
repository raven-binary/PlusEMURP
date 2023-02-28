namespace Plus.Communication.Packets.Outgoing.Notifications
{
    internal class MotdNotificationComposer : MessageComposer
    {
        public string Message { get; }

        public MotdNotificationComposer(string message)
            : base(ServerPacketHeader.MotdNotificationMessageComposer)
        {
            Message = message;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(1);
            packet.WriteString(Message);
        }
    }
}