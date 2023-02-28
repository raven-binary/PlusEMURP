namespace Plus.Communication.Packets.Outgoing.Marketplace
{
    internal class MarketplaceMakeOfferResultComposer : MessageComposer
    {
        public int Success { get; }

        public MarketplaceMakeOfferResultComposer(int success)
            : base(ServerPacketHeader.MarketplaceMakeOfferResultMessageComposer)
        {
            Success = success;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Success);
        }
    }
}