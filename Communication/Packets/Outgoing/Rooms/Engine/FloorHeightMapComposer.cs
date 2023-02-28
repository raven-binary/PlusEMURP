namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class FloorHeightMapComposer : MessageComposer
    {
        public string Map { get; }
        public int WallHeight { get; }

        public FloorHeightMapComposer(string map, int wallHeight)
            : base(ServerPacketHeader.FloorHeightMapMessageComposer)
        {
            Map = map;
            WallHeight = wallHeight;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteBoolean(true); // zoomed in
            packet.WriteInteger(WallHeight);
            packet.WriteString(Map);
        }
    }
}