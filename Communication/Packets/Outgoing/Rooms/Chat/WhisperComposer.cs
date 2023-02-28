namespace Plus.Communication.Packets.Outgoing.Rooms.Chat
{
    public class WhisperComposer : MessageComposer
    {
        public int VirtualId { get; }
        public string Text { get; }
        public int Emotion { get; }
        public int Colour { get; }

        public WhisperComposer(int virtualId, string text, int emotion, int colour)
            : base(ServerPacketHeader.WhisperMessageComposer)
        {
            VirtualId = virtualId;
            Text = text;
            Emotion = emotion;
            Colour = colour;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(VirtualId);
            packet.WriteString(Text);
            packet.WriteInteger(Emotion);
            packet.WriteInteger(Colour);

            packet.WriteInteger(0);
            packet.WriteInteger(-1);
        }
    }
}