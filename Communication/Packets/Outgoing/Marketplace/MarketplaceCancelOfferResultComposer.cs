namespace Plus.Communication.Packets.Outgoing.Marketplace
{
    internal class MarketplaceCancelOfferResultComposer : MessageComposer
    {
        public int OfferId { get; }
        public bool Success { get; }

        public MarketplaceCancelOfferResultComposer(int offerId, bool success)
            : base(ServerPacketHeader.MarketplaceCancelOfferResultMessageComposer)
        {
            OfferId = offerId;
            Success = success;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(OfferId);
            packet.WriteBoolean(Success);
        }
    }
}