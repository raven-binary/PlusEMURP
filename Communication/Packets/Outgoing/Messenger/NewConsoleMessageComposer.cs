namespace Plus.Communication.Packets.Outgoing.Messenger
{
    internal class NewConsoleMessageComposer : MessageComposer
    {
        public int Sender { get; }
        public string Message { get; }
        public int Time { get; }

        public NewConsoleMessageComposer(int sender, string message, int time = 0)
            : base(ServerPacketHeader.NewConsoleMessageMessageComposer)
        {
            Sender = sender;
            Message = message;
            Time = time;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Sender);
            packet.WriteString(Message);
            packet.WriteInteger(Time);
        }
    }
}