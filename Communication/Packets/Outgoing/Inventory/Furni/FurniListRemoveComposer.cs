namespace Plus.Communication.Packets.Outgoing.Inventory.Furni
{
    internal class FurniListRemoveComposer : MessageComposer
    {
        public int FurniId { get; }

        public FurniListRemoveComposer(int id)
            : base(ServerPacketHeader.FurniListRemoveMessageComposer)
        {
            FurniId = id;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(FurniId);
        }
    }
}