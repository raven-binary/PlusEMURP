namespace Plus.Communication.Packets.Outgoing.Rooms.Furni
{
    internal class UpdateMagicTileComposer : MessageComposer
    {
        public int ItemId { get; }
        public int Decimal { get; }

        public UpdateMagicTileComposer(int itemId, int @decimal)
            : base(ServerPacketHeader.UpdateMagicTileMessageComposer)
        {
            ItemId = itemId;
            Decimal = @decimal;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(ItemId);
            packet.WriteInteger(Decimal);
        }
    }
}