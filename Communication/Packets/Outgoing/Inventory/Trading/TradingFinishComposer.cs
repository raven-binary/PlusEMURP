namespace Plus.Communication.Packets.Outgoing.Inventory.Trading
{
    internal class TradingFinishComposer : MessageComposer
    {
        public TradingFinishComposer()
            : base(ServerPacketHeader.TradingFinishMessageComposer)
        {
        }

        public override void Compose(ServerPacket packet)
        {
        }
    }
}