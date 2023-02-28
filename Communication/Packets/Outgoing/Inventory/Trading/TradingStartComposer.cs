namespace Plus.Communication.Packets.Outgoing.Inventory.Trading
{
    internal class TradingStartComposer : MessageComposer
    {
        public int User1Id { get; }
        public int User2Id { get; }

        public TradingStartComposer(int user1Id, int user2Id)
            : base(ServerPacketHeader.TradingStartMessageComposer)
        {
            User1Id = user1Id;
            User2Id = user2Id;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(User1Id);
            packet.WriteInteger(1);
            packet.WriteInteger(User2Id);
            packet.WriteInteger(1);
        }
    }
}