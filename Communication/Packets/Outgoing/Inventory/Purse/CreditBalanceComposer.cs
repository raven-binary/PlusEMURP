namespace Plus.Communication.Packets.Outgoing.Inventory.Purse
{
    internal class CreditBalanceComposer : MessageComposer
    {
        public int CreditsBalance { get; }

        public CreditBalanceComposer(int creditsBalance)
            : base(ServerPacketHeader.CreditBalanceMessageComposer)
        {
            CreditsBalance = creditsBalance;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(CreditsBalance + ".0");
        }
    }
}