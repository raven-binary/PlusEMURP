namespace Plus.Communication.Packets.Outgoing.Rooms.Furni.LoveLocks
{
    internal class LoveLockDialogueSetLockedMessageComposer : MessageComposer
    {
        public int ItemId { get; }

        public LoveLockDialogueSetLockedMessageComposer(int itemId)
            : base(ServerPacketHeader.LoveLockDialogueSetLockedMessageComposer)
        {
            ItemId = itemId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(ItemId);
        }
    }
}