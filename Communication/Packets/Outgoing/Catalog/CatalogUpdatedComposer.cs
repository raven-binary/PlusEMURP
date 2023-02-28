namespace Plus.Communication.Packets.Outgoing.Catalog
{
    internal class CatalogUpdatedComposer : MessageComposer
    {
        public CatalogUpdatedComposer()
            : base(ServerPacketHeader.CatalogUpdatedMessageComposer)
        {
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteBoolean(false);
        }
    }
}