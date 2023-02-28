namespace Plus.Communication.Packets.Outgoing.Inventory.Trading
{
    internal class TradingErrorComposer : MessageComposer
    {
        public int Error { get; }
        public string Username { get; }

        public TradingErrorComposer(int error, string username)
            : base(ServerPacketHeader.TradingErrorMessageComposer)
        {
            Error = error;
            Username = username;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Error);
            packet.WriteString(Username);
        }
    }
}