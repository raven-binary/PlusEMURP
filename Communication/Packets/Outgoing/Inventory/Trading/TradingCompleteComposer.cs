namespace Plus.Communication.Packets.Outgoing.Inventory.Trading
{
    internal class TradingCompleteComposer : MessageComposer
    {
        public TradingCompleteComposer()
            : base(ServerPacketHeader.TradingCompleteMessageComposer)
        {
        }

        public override void Compose(ServerPacket packet)
        {
        }
    }
}