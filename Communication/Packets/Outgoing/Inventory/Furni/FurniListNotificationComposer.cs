namespace Plus.Communication.Packets.Outgoing.Inventory.Furni
{
    internal class FurniListNotificationComposer : MessageComposer
    {
        public int FurniId { get; }
        public int Type { get; }

        public FurniListNotificationComposer(int id, int type)
            : base(ServerPacketHeader.FurniListNotificationMessageComposer)
        {
            FurniId = id;
            Type = type;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(1);
            packet.WriteInteger(Type);
            packet.WriteInteger(1);
            packet.WriteInteger(Id);
        }
    }
}