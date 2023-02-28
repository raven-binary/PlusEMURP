namespace Plus.HabboHotel.Catalog.Marketplace
{
    public class MarketOffer
    {
        public int OfferId { get; }
        public int ItemType { get; }
        public int SpriteId { get; }
        public int TotalPrice { get; }
        public int LimitedNumber { get; }
        public int LimitedStack { get; }

        public MarketOffer(int offerId, int spriteId, int totalPrice, int itemType, int limitedNumber, int limitedStack)
        {
            OfferId = offerId;
            SpriteId = spriteId;
            ItemType = itemType;
            TotalPrice = totalPrice;
            LimitedNumber = limitedNumber;
            LimitedStack = limitedStack;
        }
    }
}