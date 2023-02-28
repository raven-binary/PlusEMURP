namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class RoomVisualizationSettingsComposer : MessageComposer
    {
        public int Walls { get; }
        public int Floor { get; }
        public bool HideWalls { get; }

        public RoomVisualizationSettingsComposer(int walls, int floor, bool hideWalls)
            : base(ServerPacketHeader.RoomVisualizationSettingsMessageComposer)
        {
            Walls = walls;
            Floor = floor;
            HideWalls = hideWalls;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteBoolean(HideWalls);
            packet.WriteInteger(Walls);
            packet.WriteInteger(Floor);
        }
    }
}