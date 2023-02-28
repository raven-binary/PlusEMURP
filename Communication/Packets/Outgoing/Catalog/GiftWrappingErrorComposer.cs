namespace Plus.Communication.Packets.Outgoing.Catalog
{
    internal class GiftWrappingErrorComposer : MessageComposer
    {
        public GiftWrappingErrorComposer()
            : base(ServerPacketHeader.GiftWrappingErrorMessageComposer)
        {
        }

        public override void Compose(ServerPacket packet)
        {
        }
    }
}