namespace Plus.Communication.Packets.Outgoing.Catalog
{
    internal class PurchaseErrorComposer : MessageComposer
    {
        public int ErrorCode { get; }

        public PurchaseErrorComposer(int errorCode)
            : base(ServerPacketHeader.PurchaseErrorMessageComposer)
        {
            ErrorCode = errorCode;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(ErrorCode);
        }
    }
}