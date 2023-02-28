namespace Plus.Communication.Packets.Outgoing.Marketplace
{
    internal class MarketplaceCanMakeOfferResultComposer : MessageComposer
    {
        public int Result { get; }

        public MarketplaceCanMakeOfferResultComposer(int result)
            : base(ServerPacketHeader.MarketplaceCanMakeOfferResultMessageComposer)
        {
            Result = result;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Result);
            packet.WriteInteger(0);
        }
    }
}