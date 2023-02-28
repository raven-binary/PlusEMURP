namespace Plus.Communication.Packets.Outgoing.Rooms.Furni.LoveLocks
{
    internal class LoveLockDialogueMessageComposer : MessageComposer
    {
        public int ItemId { get; }

        public LoveLockDialogueMessageComposer(int itemId)
            : base(ServerPacketHeader.LoveLockDialogueMessageComposer)
        {
            ItemId = itemId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(ItemId);
            packet.WriteBoolean(true);
        }
    }
}