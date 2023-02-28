namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class FurnitureAliasesComposer : MessageComposer
    {
        public FurnitureAliasesComposer()
            : base(ServerPacketHeader.FurnitureAliasesMessageComposer)
        {
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(0);
        }
    }
}