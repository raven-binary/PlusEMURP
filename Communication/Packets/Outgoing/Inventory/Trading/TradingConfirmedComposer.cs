namespace Plus.Communication.Packets.Outgoing.Inventory.Trading
{
    internal class TradingConfirmedComposer : MessageComposer
    {
        public int UserId { get; }
        public bool Confirmed { get; }

        public TradingConfirmedComposer(int userId, bool confirmed)
            : base(ServerPacketHeader.TradingConfirmedMessageComposer)
        {
            UserId = userId;
            Confirmed = confirmed;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(UserId);
            packet.WriteInteger(Confirmed ? 1 : 0);
        }
    }
}