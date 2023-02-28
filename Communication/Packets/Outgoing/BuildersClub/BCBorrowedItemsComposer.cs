namespace Plus.Communication.Packets.Outgoing.BuildersClub
{
    internal class BCBorrowedItemsComposer : MessageComposer
    {
        public BCBorrowedItemsComposer()
            : base(ServerPacketHeader.BCBorrowedItemsMessageComposer)
        {
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(0);
        }
    }
}