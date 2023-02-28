namespace Plus.Communication.Packets.Outgoing.Messenger
{
    internal class FindFriendsProcessResultComposer : MessageComposer
    {
        public bool Found { get; }

        public FindFriendsProcessResultComposer(bool found)
            : base(ServerPacketHeader.FindFriendsProcessResultMessageComposer)
        {
            Found = found;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteBoolean(Found);
        }
    }
}