namespace Plus.Communication.Packets.Outgoing.Rooms.FloorPlan
{
    internal class FloorPlanSendDoorComposer : MessageComposer
    {
        public int DoorX { get; }
        public int DoorY { get; }
        public int DoorDirection { get; }

        public FloorPlanSendDoorComposer(int doorX, int doorY, int doorDirection)
            : base(ServerPacketHeader.FloorPlanSendDoorMessageComposer)
        {
            DoorX = doorX;
            DoorY = doorY;
            DoorDirection = doorDirection;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(DoorX);
            packet.WriteInteger(DoorY);
            packet.WriteInteger(DoorDirection);
        }
    }
}