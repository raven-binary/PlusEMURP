namespace Plus.Communication.Packets.Outgoing.Rooms.Furni.Stickies
{
    internal class StickyNoteComposer : MessageComposer
    {
        public string ItemId { get; }
        public string ExtraData { get; }

        public StickyNoteComposer(string itemId, string extraData)
            : base(ServerPacketHeader.StickyNoteMessageComposer)
        {
            ItemId = itemId;
            ExtraData = extraData;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(ItemId);
            packet.WriteString(ExtraData);
        }
    }
}