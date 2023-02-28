namespace Plus.Communication.Packets.Outgoing.Rooms.Chat
{
    public class ShoutComposer : MessageComposer
    {
        public int VirtualId { get; }
        public string Message { get; }
        public int Emotion { get; }
        public int Colour { get; }

        public ShoutComposer(int virtualId, string message, int emotion, int colour)
            : base(ServerPacketHeader.ShoutMessageComposer)
        {
            VirtualId = virtualId;
            Message = message;
            Emotion = emotion;
            Colour = colour;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(VirtualId);
            packet.WriteString(Message);
            packet.WriteInteger(Emotion);
            packet.WriteInteger(Colour);
            packet.WriteInteger(0);
            packet.WriteInteger(-1);
        }
    }
}