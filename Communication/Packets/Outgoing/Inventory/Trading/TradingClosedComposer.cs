namespace Plus.Communication.Packets.Outgoing.Inventory.Trading
{
    internal class TradingClosedComposer : MessageComposer
    {
        public int UserId { get; }

        public TradingClosedComposer(int userId)
            : base(ServerPacketHeader.TradingClosedMessageComposer)
        {
            UserId = userId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(UserId);
            packet.WriteInteger(0);
        }
    }
}