namespace Plus.Communication.Packets.Outgoing.Misc
{
    internal class VideoOffersRewardsComposer : MessageComposer
    {
        public int OfferId { get; }
        public string Type { get; }
        public string Message { get; }

        public VideoOffersRewardsComposer(int id, string type, string message)
            : base(ServerPacketHeader.VideoOffersRewardsMessageComposer)
        {
            OfferId = id;
            Type = type;
            Message = message;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(Type);
            packet.WriteInteger(OfferId);
            packet.WriteString(Message);
            packet.WriteString("");
        }
    }
}