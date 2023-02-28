namespace Plus.Communication.Packets.Outgoing.Rooms.Freeze
{
    internal class UpdateFreezeLivesComposer : MessageComposer
    {
        public int UserId { get; }
        public int FreezeLives { get; }

        public UpdateFreezeLivesComposer(int userId, int freezeLives)
            : base(ServerPacketHeader.UpdateFreezeLivesMessageComposer)
        {
            UserId = userId;
            FreezeLives = freezeLives;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(UserId);
            packet.WriteInteger(FreezeLives);
        }
    }
}