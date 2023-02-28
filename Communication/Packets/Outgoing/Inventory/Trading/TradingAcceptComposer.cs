namespace Plus.Communication.Packets.Outgoing.Inventory.Trading
{
    internal class TradingAcceptComposer : MessageComposer
    {
        public int UserId { get; }
        public bool Accept { get; }

        public TradingAcceptComposer(int userId, bool accept)
            : base(ServerPacketHeader.TradingAcceptMessageComposer)
        {
            UserId = userId;
            Accept = accept;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(UserId);
            packet.WriteInteger(Accept ? 1 : 0);
        }
    }
}