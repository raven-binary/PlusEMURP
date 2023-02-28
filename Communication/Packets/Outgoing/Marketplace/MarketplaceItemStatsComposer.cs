namespace Plus.Communication.Packets.Outgoing.Marketplace
{
    internal class MarketplaceItemStatsComposer : MessageComposer
    {
        public int ItemId { get; }
        public int SpriteId { get; }
        public int AveragePrice { get; }

        public MarketplaceItemStatsComposer(int itemId, int spriteId, int averagePrice)
            : base(ServerPacketHeader.MarketplaceItemStatsMessageComposer)
        {
            ItemId = itemId;
            SpriteId = spriteId;
            AveragePrice = averagePrice;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(AveragePrice); //Avg price in last 7 days.
            packet.WriteInteger(PlusEnvironment.GetGame().GetCatalog().GetMarketplace().OfferCountForSprite(SpriteId));

            packet.WriteInteger(0); //No idea.
            packet.WriteInteger(0); //No idea.

            packet.WriteInteger(ItemId);
            packet.WriteInteger(SpriteId);
        }
    }
}