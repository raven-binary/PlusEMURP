namespace Plus.Communication.Packets.Outgoing.Moderation
{
    internal class BroadcastMessageAlertComposer : MessageComposer
    {
        public string Message { get; }
        public string Url { get; }

        public BroadcastMessageAlertComposer(string message, string url = "")
            : base(ServerPacketHeader.BroadcastMessageAlertMessageComposer)
        {
            Message = message;
            Url = url;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(Message);
            packet.WriteString(Url);
        }
    }
}